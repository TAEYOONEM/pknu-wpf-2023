using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using boxOffice.Logics;
using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using boxOffice.Models;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Utilities.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace boxOffice
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            string apiKey = "9d35ed6fec1f705a7bd89866d1e3fbd7";
            string targetDay = TbDate.Text.Replace("-","");
            string openApiUri = "http://kobis.or.kr/kobisopenapi/webservice/rest/boxoffice/searchDailyBoxOfficeList.json?" +
                $"key={apiKey}&targetDt={targetDay}";


            string result = string.Empty;
            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();
            }
            catch (Exception  ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            //var status = Convert.ToInt32(jsonResult["boxOfficeResult"]["dailyBoxOfficeList"]);
            try
            {
                var data = jsonResult["boxOfficeResult"]["dailyBoxOfficeList"];
                var json_array = data as JArray;

                var boxOffice = new List<BoxOffiice>();
                foreach(var movie in json_array)
                {
                    boxOffice.Add(new BoxOffiice
                    {
                        Id = 0,
                        Rnum = Convert.ToString(movie["rnum"]),
                        Rank_ = Convert.ToString(movie["rank"]),
                        RankInten = Convert.ToString(movie["rankInten"]),
                        RankOldAndNew = Convert.ToString(movie["rankOldAndNew"]),
                        MovieCd = Convert.ToString(movie["movieCd"]),
                        MovieNm = Convert.ToString(movie["movieNm"]),
                        OpenDt = Convert.ToDateTime(movie["openDt"])
                    });
                }
                this.DataContext = boxOffice;
                StsResult.Content = $"OpenAPI {boxOffice.Count}건 조회완료";
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            }
        }

        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {

            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회쫌하고 저장하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString)) 
                {
                    if(conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO boxoffice
                                            (
                                                Rnum,
                                                Rank_,
                                                RankInten,
                                                RankOldAndNew,
                                                MovieCd,
                                                MovieNm,
                                                OpenDt)
                                        VALUES
                                                (
                                                @Rnum,
                                                @Rank_,
                                                @RankInten,
                                                @RankOldAndNew,
                                                @MovieCd,
                                                @MovieNm,
                                                @OpenDt)";
                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if(temp is BoxOffiice)
                        {
                            var item = temp as BoxOffiice;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Rnum", item.Rnum);
                            cmd.Parameters.AddWithValue("@Rank_", item.Rank_);
                            cmd.Parameters.AddWithValue("@@RankInten", item.RankInten);
                            cmd.Parameters.AddWithValue("@RankOldAndNew", item.RankOldAndNew);
                            cmd.Parameters.AddWithValue("@MovieCd", item.MovieCd);
                            cmd.Parameters.AddWithValue("@MovieNm", item.MovieNm);
                            cmd.Parameters.AddWithValue("@OpenDt", item.OpenDt);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }
                    await Commons.ShowMessageAsync("저장", "DB저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB저장 오류! {ex.Message}");
            }
        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (CalDate.Visibility == Visibility.Visible)
            {
                CalDate.Visibility = Visibility.Hidden;
            }
            else
            {
                CalDate.Visibility = Visibility.Visible;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CalDate.Visibility = Visibility.Hidden;
            TbDate.Text = "";
        }

        private async void CalDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateTime.Compare((DateTime)CalDate.SelectedDate, DateTime.Now) >= 0)
            {
                await Commons.ShowMessageAsync("경고!", "현재날짜 이전의 날짜를 선택하세요.");
                return;
            }

            DateTime dt = (DateTime)CalDate.SelectedDate;
            TbDate.Text = dt.ToString("yyyy-MM-dd");
            CalDate.Visibility = Visibility.Hidden;
        }



    }
}
