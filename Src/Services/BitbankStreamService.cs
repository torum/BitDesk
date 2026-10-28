using Avalonia.Threading;
using BitDesk.Models;
using BitDesk.Services.Contracts;
using BitDesk.ViewModels;
using FluentAvalonia.UI.Controls;
using SocketIOClient;
using SocketIOClient.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BitDesk.Services;

internal class BitbankStreamService : IBitbankStreamService
{
    private SocketIOClient.SocketIO? _client;
    private string[] _tickerRooms = new[] { "ticker_btc_jpy", "ticker_eth_jpy", "ticker_xrp_jpy" };
    private string _depthRoom = string.Empty;
    private bool _isInitialized;
    private bool _isConnected;
    private bool _isDisconnecting;
    private readonly Lock _lock = new();

    public event EventHandler<BitbankTickerData>? TickerUpdated;
    public event EventHandler<BitbankDepthData>? DepthUpdated;
    public event EventHandler<ErrorInfo>? ErrorOccured;

    public BitbankStreamService()
    {
    }

    public bool IsConnected => _isConnected;

    public void Initialize(List<PairCodes> pairCodes)
    {
        _isInitialized = true;
        _tickerRooms = pairCodes.Select(pc => $"ticker_{pc.ToString().ToLower()}").ToArray();
    }

    public async Task StartAsync()
    {
        if (!_isInitialized)
        {
            Debug.WriteLine("--> BitbankStreamService is not initialized. Call Initialize() before StartAsync().");
            return;
        }

        if (_isConnected) return;

        // Bitbank Socket.IO endpoint (Engine.IO v4)
        var uri = new Uri("wss://stream.bitbank.cc");
        var options = new SocketIOOptions
        {
            EIO = EngineIO.V4,
            Transport = TransportProtocol.WebSocket,

            ConnectionTimeout = TimeSpan.FromSeconds(4),

            Reconnection = true,
            ReconnectionAttempts = 3,
        };

        _client = new SocketIOClient.SocketIO(uri, options);

        _client.OnConnected += OnClientConnected;

        _client.OnDisconnected += (sender, e) =>
        {
            Debug.WriteLine($"--> Disconnected: {e}");

            if (_isDisconnecting)
            {
                return;
            }

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = e,
                ErrorSeverity = FAInfoBarSeverity.Informational
            };

            ErrorOccured?.Invoke(this, errorInfo);
        };

        _client.OnError += (sender, errorDescription) =>
        {
            Debug.WriteLine($"Socket.IO Error occurred: {errorDescription}");

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = errorDescription,
                ErrorSeverity = FAInfoBarSeverity.Warning
            };

            ErrorOccured?.Invoke(this, errorInfo);
        };

        _client.On("message", response =>
        {
            try
            {
                var element = response.GetValue<System.Text.Json.JsonElement?>(0);
                if (!element.HasValue) return Task.CompletedTask;

                // Prints the raw JSON string for debugging purposes
                //string rawJsonString = element.Value.GetRawText();
                //Debug.WriteLine($"[RAW STREAM DATA]: {rawJsonString}");

                /*
                var data = root.Message.Data;
                string pair = (root.RoomName ?? "unknown").Replace("ticker_", "").ToUpper();
                DateTime timeStamp = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime;

                // Calculate a quick math metric (e.g., daily percentage change)
                decimal priceChange = data.Last - data.Open;
                decimal percentChange = data.Open != 0 ? (priceChange / data.Open) * 100 : 0;

                // Print out your typed metrics using standard number formatting
                Debug.WriteLine($"=== {pair} NUMERIC DATA [{timeStamp:yyyy-MM-dd HH:mm:ss}] ===");
                Debug.WriteLine($"  Last Price : {data.Last:N0} JPY ({percentChange:+0.00;-0.00;0.00}%)");
                Debug.WriteLine($"  Bid (Buy)  : {data.Buy:N0}   | Ask (Sell) : {data.Sell:N0}");
                Debug.WriteLine($"  24h High   : {data.High:N0}  | 24h Low    : {data.Low:N0}");
                Debug.WriteLine($"  24h Volume : {data.Vol:N4}");
                Debug.WriteLine(new string('=', 45));
                */

                if (!element.HasValue || element.Value.ValueKind != JsonValueKind.Object)
                {
                    Debug.WriteLine("--> Received message that is not a JSON object. Ignoring.");
                    return Task.CompletedTask;
                }

                // Access the "room_name" property directly from the root object
                if (!element.Value.TryGetProperty("room_name", out JsonElement roomProperty) || roomProperty.ValueKind != JsonValueKind.String)
                {
                    Debug.WriteLine("--> Received message without a valid 'room_name'. Ignoring.");
                    return Task.CompletedTask;
                }

                string roomName = roomProperty.GetString() ?? string.Empty;

                if (roomName.StartsWith("ticker_"))
                {
                    var root = response.GetValue<BitbankTickerRoot>(0);

                    if (root?.Message?.Data != null)
                    {
                        ProcessTickerData(root);
                    }
                }
                else if (roomName.StartsWith("depth_whole_"))
                {
                    var root = response.GetValue<BitbankDepthRoot>(0);

                    if (root?.Message?.Data != null)
                    {
                        ProcessDepthData(root);
                    }
                }
                else
                {
                    Debug.WriteLine($"--> Received data for an unhandled room: {roomName}.");
                    return Task.CompletedTask;
                }

                /*
                var element = response.GetValue<System.Text.Json.JsonElement?>(0);
                if (!element.HasValue) return Task.CompletedTask;

                // Prints the raw JSON string for debugging purposes
                //string rawJsonString = element.Value.GetRawText();
                //Debug.WriteLine($"[RAW STREAM DATA]: {rawJsonString}");

                var root = response.GetValue<BitbankTickerRoot>(0);

                // Ensure data nodes exist before reading values
                if (root?.Message?.Data != null)
                {
                    string pair = (root.RoomName ?? "unknown").Replace("ticker_", "");

                    if (Enum.TryParse(pair, true, out PairCodes pairCode))
                    {
                        var data = root.Message.Data with { TickerPair = pairCode };
                        OnTickerUpdated(data);
                    }
                    else
                    {
                        Debug.WriteLine($"--> Unknown pair code received: {pair}");
                    }
                }
                */
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--> Exception while processing message: {ex}");

                var errorInfo = new ErrorInfo(ErrType.ResposeParseError)
                {
                    ErrorMessage = $"{ex.Message} ({ex.GetType().FullName})",
                    ErrorSeverity = FAInfoBarSeverity.Warning
                };

                ErrorOccured?.Invoke(this, errorInfo);
            }

            return Task.CompletedTask;
        });

        //await _client.ConnectAsync();
        try
        {
            await _client.ConnectAsync();
        }
        catch (SocketIOClient.Exceptions.ConnectionException ex)
        {
            Debug.WriteLine($"--> SocketIOClient.Exceptions.ConnectionException ConnectAsync() @BitbankStreamService: {ex.Message}");

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = ex.Message,
                ErrorSeverity = FAInfoBarSeverity.Warning
            };

            ErrorOccured?.Invoke(this, errorInfo);
        }
        catch (System.TimeoutException ex)
        {
            Debug.WriteLine($"--> System.TimeoutException ConnectAsync() @BitbankStreamService: {ex.Message}");

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = ex.Message,
                ErrorSeverity = FAInfoBarSeverity.Warning
            };

            ErrorOccured?.Invoke(this, errorInfo);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"--> Failed to connect. @BitbankStreamService: {ex}");

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = $"{ex.Message} ({ex.GetType().FullName})",
                ErrorSeverity = FAInfoBarSeverity.Warning
            };

            ErrorOccured?.Invoke(this, errorInfo);
        }
    }

    private async void OnClientConnected(object? sender, EventArgs e)
    {
        if (_client == null) return;

        _isConnected = true;

        Debug.WriteLine("--> Successfully connected to Bitbank Stream! Subscribing now...");

        try
        {
            if (_tickerRooms.Length == 0) return;

            foreach (var room in _tickerRooms)
            {
                var roomArray = new[] { room };
                await _client.EmitAsync("join-room", roomArray);
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"--> Failed to join rooms @BitbankStreamService: {ex}");

            var errorInfo = new ErrorInfo(ErrType.ResposeParseError)
            {
                ErrorMessage = $"{ex.Message} ({ex.GetType().FullName}) @OnClientConnected.",
                ErrorSeverity = FAInfoBarSeverity.Warning
            };
            ErrorOccured?.Invoke(this, errorInfo);
        }
    }

    private void ProcessTickerData(BitbankTickerRoot root)
    {
        /*
        var data = root.Message.Data;
        string pair = (root.RoomName ?? "unknown").Replace("ticker_", "").ToUpper();
        DateTime timeStamp = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime;

        // Calculate a quick math metric (e.g., daily percentage change)
        decimal priceChange = data.Last - data.Open;
        decimal percentChange = data.Open != 0 ? (priceChange / data.Open) * 100 : 0;

        // Print out your typed metrics using standard number formatting
        Debug.WriteLine($"=== {pair} NUMERIC DATA [{timeStamp:yyyy-MM-dd HH:mm:ss}] ===");
        Debug.WriteLine($"  Last Price : {data.Last:N0} JPY ({percentChange:+0.00;-0.00;0.00}%)");
        Debug.WriteLine($"  Bid (Buy)  : {data.Buy:N0}   | Ask (Sell) : {data.Sell:N0}");
        Debug.WriteLine($"  24h High   : {data.High:N0}  | 24h Low    : {data.Low:N0}");
        Debug.WriteLine($"  24h Volume : {data.Vol:N4}");
        Debug.WriteLine(new string('=', 45));
        */

        if (string.IsNullOrWhiteSpace(root.RoomName))
        {
            Debug.WriteLine("--> Received message with empty RoomName. Ignoring.");
            return;
        }

        if (root.RoomName.StartsWith("ticker_"))
        {
            string pair = (root.RoomName ?? "unknown").Replace("ticker_", "");
            if (Enum.TryParse(pair, true, out PairCodes pairCode))
            {
                var data = root.Message.Data with { TickerPair = pairCode };
                OnTickerUpdated(data);
            }
            else
            {
                Debug.WriteLine($"--> Unknown pair code received: {pair}.");
                return;
            }
        }
        else
        {
            Debug.WriteLine($"--> Received message for unknown room type: {root.RoomName}. Ignoring.");
            return;
        }
    }

    private void ProcessDepthData(BitbankDepthRoot root)
    {
        if (string.IsNullOrWhiteSpace(root.RoomName))
        {
            Debug.WriteLine("--> Received message with empty RoomName. Ignoring.");
            return;
        }

        if (root.RoomName.StartsWith("depth_whole_"))
        {
            string pair = (root.RoomName ?? "").Replace("depth_whole_", "");
            if (Enum.TryParse(pair, true, out PairCodes pairCode))
            {
                var data = root.Message.Data with { DepthPair = pairCode };
                OnDepthUpdated(data);
            }
            else
            {
                Debug.WriteLine($"--> Unknown pair code received: {pair}.");
                return;
            }
        }
        else
        {
            Debug.WriteLine($"--> Received message for unknown room type: {root.RoomName}. Ignoring.");
            return;
        }
    }

    public async Task JoinTickerRoomAsync(PairCodes pairCode)
    {
        if (_client == null || !_isConnected) return;

        if (!_isInitialized) return;

        if (string.IsNullOrWhiteSpace(pairCode.ToString())) return;

        if (_tickerRooms.Contains($"ticker_{pairCode.ToString().ToLower()}", StringComparer.OrdinalIgnoreCase)) return;

        Debug.WriteLine($"--> Joining Bitbank Stream rooms...ticker_{pairCode}.");

        // Wrap the room string in an array, matching your subscription pattern
        var roomArray = new[] { $"ticker_{pairCode.ToString().ToLower()}" };

        // Emit 'join-room' to tell the Bitbank server to start streaming this data
        await _client.EmitAsync("join-room", roomArray);

        lock (_lock)
        {
            List<string> roomList = [.. _tickerRooms];
            roomList.Add($"ticker_{pairCode.ToString().ToLower()}");
            _tickerRooms = [.. roomList];
        }

        Debug.WriteLine($"--> Joined room: ticker_{pairCode}");
    }

    public async Task LeaveTickerRoomAsync(PairCodes pairCode)
    {
        if (_client == null || !_isConnected) return;

        if (!_isInitialized) return;

        string room = $"ticker_{pairCode.ToString().ToLower()}";

        if (string.IsNullOrWhiteSpace(room)) return;

        if (!_tickerRooms.Contains(room, StringComparer.OrdinalIgnoreCase)) return;

        Debug.WriteLine($"--> Leaving Bitbank Stream rooms...{room}.");

        // Wrap the room string in an array, matching your subscription pattern
        var roomArray = new[] { room };

        // Emit 'leave-room' to tell the Bitbank server to stop streaming this data
        await _client.EmitAsync("leave-room", roomArray);

        lock (_lock)
        {
            List<string> roomList = [.. _tickerRooms];
            roomList.Remove(room);
            _tickerRooms = [.. roomList];

        }
        Debug.WriteLine($"--> Left room: {room}");
    }

    public async Task JoinDepthRoomAsync(PairCodes pairCode)
    {
        if (_client == null || !_isConnected) return;

        if (!_isInitialized) return;

        if (string.IsNullOrWhiteSpace(pairCode.ToString())) return;

        if (_depthRoom.Equals($"depth_whole_{pairCode.ToString().ToLower()}", StringComparison.OrdinalIgnoreCase))
        {
            Debug.WriteLine($"--> Already joined depth room: {_depthRoom}. No action taken.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(_depthRoom))
        {
            Debug.WriteLine($"--> Already joined depth room: {_depthRoom}. Leaving it first...");

            string pair = (_depthRoom ?? "").Replace("depth_whole_", "");
            if (!string.IsNullOrWhiteSpace(pair))
            {
                if (Enum.TryParse(pair, true, out PairCodes currentPairCode))
                {
                    await LeaveDepthRoomAsync(currentPairCode);
                }
                else
                {
                    Debug.WriteLine($"--> Unknown pair code received for depth room: {pair}");
                }
            }
            else
            {
                Debug.WriteLine($"--> Current depth room is empty or invalid. Cannot leave.");
                return;
            }
        }

        Debug.WriteLine($"--> Joining Bitbank Stream rooms...depth_whole_{pairCode}.");

        // Wrap the room string in an array, matching your subscription pattern
        var roomArray = new[] { $"depth_whole_{pairCode.ToString().ToLower()}" };

        // Emit 'join-room' to tell the Bitbank server to start streaming this data
        await _client.EmitAsync("join-room", roomArray);

        lock (_lock)
        {
            _depthRoom = $"depth_whole_{pairCode.ToString().ToLower()}";
        }

        Debug.WriteLine($"--> Joined room: {_depthRoom}");
    }

    public async Task LeaveDepthRoomAsync(PairCodes pairCode)
    {
        if (_client == null || !_isConnected) return;

        if (!_isInitialized) return;

        string room = $"depth_whole_{pairCode.ToString().ToLower()}";

        if (string.IsNullOrWhiteSpace(room)) return;

        if (!_depthRoom.Equals(room, StringComparison.OrdinalIgnoreCase))
        {
            Debug.WriteLine($"--> Not currently in depth room: {room}. Current depth room: {_depthRoom}");
            return;
        }

        Debug.WriteLine($"--> Leaving Bitbank Stream room...{room}");

        // Wrap the room string in an array, matching your subscription pattern
        var roomArray = new[] { room };

        // Emit 'leave-room' to tell the Bitbank server to stop streaming this data
        await _client.EmitAsync("leave-room", roomArray);

        lock (_lock)
        {
            _depthRoom = string.Empty;
        }

        Debug.WriteLine($"--> Left room: {room}");
    }

    public async Task StopAsync()
    {
        if (_isDisconnecting) return;
        _isDisconnecting = true;

        _client?.OnConnected -= OnClientConnected;

        if (_client is not null && _isConnected == true)
        {
            try
            {
                if (_client.Connected)
                {
                    Debug.WriteLine("--> Disconnecting from Bitbank Stream...");
                    await _client.DisconnectAsync(); // 1. Signal graceful disconnect to server
                }

                _client?.Dispose();
            }
            catch (System.Net.WebSockets.WebSocketException)
            {
                /* Swallow any concurrent connection drops */
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--> Exception during disconnect: {ex}");
            }
            finally
            {
                _isConnected = false;
            }
        }
    }

    protected virtual void OnTickerUpdated(BitbankTickerData data)
    {
        TickerUpdated?.Invoke(this, data);
    }

    protected virtual void OnDepthUpdated(BitbankDepthData data)
    {
        DepthUpdated?.Invoke(this, data);
    }
}
