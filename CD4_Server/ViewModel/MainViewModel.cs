using CD4_Server.Communications;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using CD4_Server.DataHandling;
using System;
using System.Linq;

namespace CD4_Server.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        private Server server;
        private const int port = 8010;
        private const string ip = "127.0.0.1";
        private bool isConnected = false;

        #region properties for GUI 
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public string SelectedUser { get; set; }
        public RelayCommand BtnStartClicked { get; set; }
        public RelayCommand BtnStopClicked { get; set; }
        public RelayCommand BtnDropClicked { get; set; }
        public int NoOfReceivedMessages { get { return Messages.Count; } }

        //LogFiles
        private DataHandler dataHandler;
        public ObservableCollection<string> LogFiles
        {
            get { return new ObservableCollection<string>(dataHandler.GetAllLogFiles()); }
        }
        public string SelectedLogFile { get; set; }
        public ObservableCollection<string> LogFileMessages { get; set; }
        public RelayCommand BtnSaveLogFileClicked { get; set; }
        public RelayCommand BtnShowLogFileClicked { get; set; }
        public RelayCommand BtnDropLogFileClicked { get; set; }
        #endregion

        public MainViewModel()
        {
            Users = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();

            //Server initialisieren und Thread für das Akzeptieren starten
            BtnStartClicked = new RelayCommand(
                ()=>
                {
                    //Server nicht im DesignMode initialisieren (Stichwort: Portsperre)
                    if (!IsInDesignMode) 
                    {
                        server = new Server(ip, port, UpdateGuiWithNewMessage);
                        server.StartAccepting();
                    }

                    isConnected = true;
                },
                () => { return !isConnected; });

            //Thread für das Akzeptieren stoppen
            BtnStopClicked = new RelayCommand(
                ()=>
                {
                    server.StopAccepting();
                    isConnected = false;
                },
                () => { return isConnected; });

            //spezifischen Client trennen und aus Liste löschen
            BtnDropClicked = new RelayCommand(
                ()=>
                {
                    server.DisconnectSpecificClient(SelectedUser);
                    Users.Remove(SelectedUser);
                },
                ()=> { return SelectedUser != null; });

            //LogFiles
            dataHandler = new DataHandler();

            //LogFiles speichern
            BtnSaveLogFileClicked = new RelayCommand(
                () =>
                {
                    dataHandler.SaveLogFile(Messages.ToList());
                    RaisePropertyChanged("LogFiles");
                },
                ()=> { return Messages != null; });

            //Messages aus LogFiles laden und anzeigen
            BtnShowLogFileClicked = new RelayCommand(
                () =>
                {
                    LogFileMessages = new ObservableCollection<string>(dataHandler.GetAllMessagesFromLogFile(SelectedLogFile));
                    RaisePropertyChanged("LogFileMessages");
                },
                () => { return SelectedLogFile != null; });

            //LogFiles löschen
            BtnDropLogFileClicked = new RelayCommand(
                () =>
                {
                    dataHandler.DeleteLogFiles(SelectedLogFile);
                    RaisePropertyChanged("LogFiles");
                },
                () => { return SelectedLogFile != null; });
        }

        //Messages in die GUI schreiben (Sender: Nachricht)
        public void UpdateGuiWithNewMessage(string message)
        {
            //Thread zu GUI-Main Thread wechseln, damit man in die GUI schreiben kann
            App.Current.Dispatcher.Invoke(() =>
            {
                //message = "Kati: Hallo";
                //den Namen aus der Message entnehmen
                string name = message.Split(':')[0];

                //falls ein (neuer) User noch nicht in der Liste ist, in Liste aufnehmen
                if (!Users.Contains(name)) 
                {
                    Users.Add(name);
                }

                //Nachricht in die Observ.Coll. Messages hinzufügen und Anzahl an Nachrichten updaten
                Messages.Add(message); 
                RaisePropertyChanged("NoOfReceivedMessages");
            });
        }
    }
}