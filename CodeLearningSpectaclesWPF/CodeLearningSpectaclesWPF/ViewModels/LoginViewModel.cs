using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeLearningSpectaclesWPF.ViewModels
{
  internal class LoginViewModel : INotifyPropertyChanged
  {

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _urlMessage;
    public string UrlMessage
    {
      get
      {
        return _urlMessage;
      } 
      
      set
      {
        _urlMessage = value;
        NotifyPropertyChanged();
      }

    }
    private string _messageStart;
    public string MessageStart
    { 
      get
      {
        return _messageStart;
      }
      set
      {
        _messageStart = value;
        NotifyPropertyChanged();
      }
    }
    private string _messageEnd;
    public string MessageEnd
    { 
      get
      {
        return _messageEnd;
      }
      set
      {
        _messageEnd = value;
        NotifyPropertyChanged();
      }
    }

    public LoginViewModel()
    {
      UrlMessage = "";
      MessageStart = "";
      MessageEnd = "";
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

  }
}
