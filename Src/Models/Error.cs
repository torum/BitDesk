using CommunityToolkit.Mvvm.ComponentModel;

namespace BitDesk.Models;

// APIリクエストの結果に含めて返すエラー情報保持クラス
public partial class ErrorInfo : ObservableObject
{
    private string _errorTitle = string.Empty;
    public string ErrorTitle
    {
        get => _errorTitle;
        set
        {
            if (_errorTitle == value)
            {
                return;
            }

            _errorTitle = value;
            OnPropertyChanged(nameof(ErrorTitle));
        }
    }

    private int _errorCode;
    public int ErrorCode
    {
        get => _errorCode;
        set
        {
            if (_errorCode == value)
            {
                return;
            }

            _errorCode = value;
            OnPropertyChanged(nameof(ErrorCode));
        }
    }

    private string _errorDescription = string.Empty;
    public string ErrorDescription
    {
        get => _errorDescription;
        set
        {
            if (_errorDescription == value)
            {
                return;
            }

            _errorDescription = value;
            OnPropertyChanged(nameof(ErrorDescription));
        }
    }

}

