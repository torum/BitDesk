using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using System.Collections.Generic;

namespace BitDesk.Models;

public enum ErrType
{
    ConnectionError, HttpError, ApiError, ResposeParseError, UserInputInvalid, Unexpected, Other
};

public partial class ErrorInfo : ObservableObject
{
    private ErrType errType;

    public ErrorInfo(ErrType errType)
    {
        this.errType = errType;
    }

    private Dictionary<ErrType, string> ErrTypeDictionary
    {
        get;
    } = new Dictionary<ErrType, string>()
        {
            {ErrType.ConnectionError, "Connection Error"},
            {ErrType.HttpError, "Http Error"},
            {ErrType.ApiError, "API Error"},
            {ErrType.ResposeParseError, "Respose Parse Error"},
            {ErrType.UserInputInvalid, "Invalid Input Value"},
            {ErrType.Unexpected, "Unexpected Error"},
            {ErrType.Other, "Error"},
        };

    public ErrType ErrorType
    {
        get
        {
            return errType;
        }
        set
        {
            if (errType == value)
            {
                return;
            }
            errType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ErrorTitle));
        }
    }

    public string ErrorTitle
    {
        get => ErrTypeDictionary[errType];
    }

    public int ErrorCode
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    public FAInfoBarSeverity ErrorSeverity
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = FAInfoBarSeverity.Warning;

    public string ErrorMessage
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

}

