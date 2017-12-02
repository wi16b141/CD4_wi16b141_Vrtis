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

        #region properties for GUI
        public string ChatName { get; set; }
        public string Message { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand BtnConnectClicked { get; set; }
        public RelayCommand BtnSendClicked { get; set; }
        #endregion

        public MainViewModel()
        {
            Messages = new ObservableCollection<string>();

            //Verbindung mit dem Server initialisieren
            BtnConnectClicked = new RelayCommand(
                () =>
                {
                    isConnected = true; //(Connect Button greift auf diese Variable)
                    client = new Clients(ip, port, new Action<string>(NewMessageReceived),ClientDissconnected);
                },
                ()=> { return (!isConnected && ChatName != null); });

            //Nachrichten an den Server und die eigene GUI schicken
            BtnSendClicked = new RelayCommand(
                () =>
                {
                    client.Send(ChatName + ": " + Message);
                    Messages.Add("YOU: " + Message); //Anzeige in der eigenen GUI
                },
                () => { return (isConnected && Message != null); });
        }

        //ändert die Variable isConnected, worauf Connect-Button greift, auf false ab
        private void ClientDissconnected() 
        {
            isConnected = false;

            //den CommandManager zwingen, alle Konditionen auf Änderungen zu überprüfen
            CommandManager.InvalidateRequerySuggested(); 
        }

        private void NewMessageReceived(string message)
        {
            //um Nachrichten aus anderen Threads in die GUI zu schreiben
            App.Current.Dispatcher.Invoke(() =>
            {
                //fügt fremde Nachricht in die Observ.Coll. Messages hinzu
                Messages.Add(message); 
            });
        }
    }
}