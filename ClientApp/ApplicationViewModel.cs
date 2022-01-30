using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using System.Windows.Input;
using System.Collections.Generic;

namespace ClientApp
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MyFile selectedFile; //выбранный файл для отправки на сервер
        private string answerText;   //ответы от сервера
        private Client ClientObject;

        private IFileService FileService;
        private IDialogService DialogService;

        public ICommand OpenCommand { get; }   //команда открытия файлов 
        public ICommand SendCommand { get; }   //команда отправки содержимого выбранного файла на сервер
        public ICommand DeleteCommand { get; } //команда удаления файла из списка открытых

        public ICommand ClearCommand { get; }  //команда очистки ответов сервера


        public ObservableCollection<MyFile> TextFiles { get; set; }

        public ApplicationViewModel(IDialogService dialogServ, IFileService fileServ, Client client)
        {
            DialogService = dialogServ;
            FileService = fileServ;
            TextFiles = new ObservableCollection<MyFile>();

            ClientObject = client;

            OpenCommand = new RelayCommand(OpenFile);
            SendCommand = new RelayCommand(SendFileAsync);
            ClearCommand = new RelayCommand(ClearAnswerText);
            DeleteCommand = new RelayCommand(DeleteFile);
        }
        public MyFile SelectedFile
        {
            get => selectedFile;
            set
            {
                selectedFile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedText"));
            }
        }
        public string AnswerText
        {
            get => answerText;
            set
            {
                answerText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AnswerText"));
            }
        }
        //открытие файлов
        private void OpenFile()
        {
            string text, path;

            if (DialogService.OpenFileDialog())
            {
                foreach (var item in DialogService.FilePaths)
                {
                    text = FileService.Open(item);
                    path = item.ToString();
                    TextFiles.Add(new MyFile(text, path));
                }
            }
        }
        //отправка файла на сервер
        private async void SendFileAsync()
        {
            if (SelectedFile != null)
            {
                MyFile temp = SelectedFile;
                string answer = await ClientObject.ExchangeAsync(SelectedFile);
                AnswerText += temp.FilePath + ":\t" + answer + "\n";
                if (answer != "Сервер перегружен") TextFiles.Remove(temp); //TO DO
            }
        }

        //удаление файла из списка открытых
        private void DeleteFile()
        {
            if (SelectedFile != null)
            {
                TextFiles.Remove(SelectedFile);
            }
        }
        private void ClearAnswerText()
        {
            AnswerText = string.Empty;
        }
    }
}
