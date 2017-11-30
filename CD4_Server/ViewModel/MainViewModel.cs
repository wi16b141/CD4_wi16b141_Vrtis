using CD4_Server.Communications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;

namespace CD4_Server.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        private Server server;
        private const int port = 8010;
        private const string ip = "127.0.0.1";
        private bool isConnected = false;
        
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public string SelectedUser { get; set; }

        //Buttons
        public RelayCommand BtnStartClicked { get; set; }
        public RelayCommand BtnStopClicked { get; set; }
        public RelayCommand BtnDropClicked { get; set; }
        //public RelayCommand BtnSaveClicked { get; set; }

        public int NoOfReceivedMessages { get { return Messages.Count; } }

        public MainViewModel()
        {
            Users = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            BtnStartClicked = new RelayCommand(BtnStart, () => { return !isConnected; });
            BtnStopClicked = new RelayCommand(BtnStop, () => { return isConnected; });
            BtnDropClicked = new RelayCommand(BtnDrop, ()=> { return SelectedUser != null; });
        }

        private void BtnDrop()
        {
            server.DisconnectSpecificClient(SelectedUser);
            Users.Remove(SelectedUser);
        }

        private void BtnStop()
        {
            server.StopAccepting();
            isConnected = false;
        }

        private void BtnStart()
        {
            server = new Server(ip, port, UpdateGuiWithNewMessage);
            server.StartAccepting();
            isConnected = true;
        }

        public void UpdateGuiWithNewMessage(string message)
        {
            //switch thread to GUI thread to write to GUI
            App.Current.Dispatcher.Invoke(() =>
            {
                string name = message.Split(':')[0];

                if (!Users.Contains(name)) //falls ein (neuer) User noch nicht in der Liste ist
                {
                    Users.Add(name);
                }
                
                Messages.Add(message); //Nachricht verfassen
                RaisePropertyChanged("NoOfReceivedMessages"); //Anzahl an Nachrichten updaten
            });
        }
    }
}