using CD4_Clien.Communications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;

namespace CD4_Clien.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Clients client;
        private bool isConnected;
        private const string ip = "127.0.0.1";
        private const int port = 8010;

        public string ChatName { get; set; }
        public string Message { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand BtnConnectClicked { get; set; }
        public RelayCommand BtnSendClicked { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<string>();
            BtnConnectClicked = new RelayCommand(BtnConnect, ()=> { return (!isConnected && ChatName != null); });
            BtnSendClicked = new RelayCommand(BtnSend, () => { return (isConnected && Message != null); });
        }

        private void BtnSend()
        {
            client.Send(ChatName + ": " + Message);
            Messages.Add("YOU: " + Message); //Anzeige in der eigenen GUI
        }

        private void BtnConnect()
        {
            isConnected = true;
            client = new Clients(ip, port, new Action<string>(NewMessageReceived), ClientDissconnected);
        }

        private void ClientDissconnected()
        {
            isConnected = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private void NewMessageReceived(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(message); //fügt fremde Nachricht in die OC Messages hinzu
            });
        }
    }
}