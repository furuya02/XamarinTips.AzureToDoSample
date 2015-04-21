using System;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

namespace AzureToDoSample
{
    public class App : Application
    {
        public App(){
            if (Device.OS == TargetPlatform.Android){
                //Androidでインジケータ(IsBusy)を表示するためにNavigationPageを使用する
                MainPage = new NavigationPage(new MyPage());
            }else{
                MainPage = new MyPage();
            }
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

    class MyPage : ContentPage{

        //モバイルサービスへの接続するためのURLとアクセスキー
        const string ApplicationUrl = "https://xxxxxxxxxxxxxxxx.azure-mobile.net/";　
        const string ApplicationKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"; 
        readonly MobileServiceClient _client = new MobileServiceClient(ApplicationUrl, ApplicationKey);  

        //リストボックスのデータソース
        readonly ObservableCollection<ToDoItem> _ar = new ObservableCollection<ToDoItem>();

        public MyPage(){
            var entry = new Entry{
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Placeholder = "ここへ入力してください",
            };
            var buttonAdd = new Button{
                Text = "追加",
                WidthRequest = 60,
            };
            buttonAdd.Clicked += (s, a) => Add(entry); 


            var listView = new ListView() {
                ItemsSource = _ar,
                ItemTemplate = new DataTemplate(typeof (TextCell)),
            };
            listView.ItemTemplate.SetBinding(TextCell.TextProperty, "Text");
            listView.ItemTapped += async (s, a) =>
            {
                //タップ時のイベント処理
                if (await DisplayAlert("確認", "削除して宜しいですか?", "OK", "Cancel"))
                {
                    Del((ToDoItem)a.Item);
                };
            };



            Content = new StackLayout{
                Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
                Children = {
                    new StackLayout {
                        Padding = new Thickness(5),
                        Orientation = StackOrientation.Horizontal,
                        Children = {entry, buttonAdd }
                    },
                    listView
                }
            };

            Refresh();//Azure上のデータで表示を更新する
        }
        //表示更新
        async void Refresh()
        {
            IsBusy = true;//インジケータのON

            _ar.Clear();

            //DB検索 (Complete==falseのデータだけ抽出する)
            var result = await _client.GetTable<ToDoItem>().Where(item => item.Complete == false).ToListAsync();
            result.Reverse();
            foreach (var a in result)
            {
                _ar.Add(a);
            }
            IsBusy = false;//インジケータのOFF
        }


        //削除
        private async void Del(ToDoItem item)
        {
            IsBusy = true;//インジケータのON

            //DB更新
            item.Complete = true;
            await _client.GetTable<ToDoItem>().UpdateAsync(item);

            //リストボックスからの削除
            _ar.Remove(item);
            IsBusy = false;//インジケータのOFF
        }

        //追加
        async void Add(Entry entry)
        {
            if (!String.IsNullOrEmpty(entry.Text))
            {
                IsBusy = true;//インジケータのON

                var item = new ToDoItem { Complete = false, Text = entry.Text };
                await _client.GetTable<ToDoItem>().InsertAsync(item);

                _ar.Insert(0, item);
                entry.Text = "";
                IsBusy = false;//インジケータのOFF
            }
        }

    }
}
