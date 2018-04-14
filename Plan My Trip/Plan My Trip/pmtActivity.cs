using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Timers;

namespace Plan_My_Trip
{
    [Activity(Label = "pmtActivity")]
    public class pmtActivity : Activity
    {
        String previous_dest = "";
        //string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        //string filename = "";
        ListView lstdisplay;
        ArrayAdapter<string> adapter;
        Button report;
        int id = 0;
        bool click = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.pmt);
            // Create your application here
            string strid = Intent.GetStringExtra("text");
            id = Convert.ToInt32(strid);
           // filename = Path.Combine(path, "current.data");
            lstdisplay = FindViewById<ListView>(Resource.Id.listViewdisplay);
            List<string> prevtrip = new List<string>();
            report = FindViewById<Button>(Resource.Id.buttonreport);
            report.Click += Report_Click;
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/" + id.ToString() + "/destname.pmt");
                if (stream != null)
                {
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadLine().Trim();
                    reader.Close();
                    if (!content.Contains('<'))
                    {
                        adapter.Add("Your destination is " + content);
                    }
                }
                stream.Close();
            } catch (Exception) { }
            Update();
            /* try {
                 WebClient client = new WebClient();
                 Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/" + id.ToString()+"/current.pmt");
                 if (stream != null)
                 {
                     StreamReader reader = new StreamReader(stream);
                     while (!reader.EndOfStream)
                     {
                         prevtrip.Add(reader.ReadLine());
                     }
                     reader.Close();

                     Android.Widget.Toast.MakeText(this, "Loaded trip data", ToastLength.Short).Show();
                 }
                 stream.Close();
                 if (prevtrip.Count > 0)
                 {
                     adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, prevtrip.ToArray());
                     lstdisplay.Adapter = adapter;
                 }
                 else


             } catch (Exception) { }
             */

        }

        private void Report_Click(object sender, EventArgs e)
        {
            click = true;
            var intent = new Intent(this, typeof(reportActivity));
            intent.PutExtra("text", id.ToString());
            StartActivity(intent);      
        }

        private async void Update()
        {
            while (click == false) {
                await Task.Delay(2000);     
                    try
                    {
                        string response = new WebClient().DownloadString("http://www.planmytrip.net23.net/run.php?id=" + id.ToString());
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/" + id.ToString()+"/run.pmt");
                        StreamReader reader = new StreamReader(stream);
                        string content = reader.ReadLine().Trim();
                        stream.Close();
                        reader.Close();
                        switch (content.ToLower())
                        {
                            case "f":
                            try
                            {
                                stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/" + id.ToString() + "/location.pmt");
                                if(stream != null)
                                {
                                    reader = new StreamReader(stream);
                                    content = reader.ReadLine().Trim();
                                    reader.Close();
                                    if (content != previous_dest && !content.Contains('<'))
                                    {                        
                                        Android.Widget.Toast.MakeText(this, "Your current location is " + content, ToastLength.Short).Show();
                                        if (previous_dest != "")
                                        {
                                            Android.Widget.Toast.MakeText(this, "Your previous location was " + previous_dest, ToastLength.Short).Show();
                                            adapter.Add("Your previous location is " + previous_dest.Trim() + "  =>last updated " + DateTime.Now.ToShortTimeString());
                                        }
                                        adapter.Add("Your current location is " + content + "  =>last updated " + DateTime.Now.ToShortTimeString());
                                        previous_dest = content;
                                        content = "";
                                        lstdisplay.Adapter = adapter;
                                    }
                                }
                                stream.Close();

                            }
                            catch(Exception)
                            {
                               Android.Widget.Toast.MakeText(this, "Error: No gps device", ToastLength.Short).Show();
                            }
                            break;
                            case "ng":
                                Android.Widget.Toast.MakeText(this, "Error: GPS signal low", ToastLength.Short).Show();
                            break;
                            default:
                                Android.Widget.Toast.MakeText(this, "Error: Hardware system down", ToastLength.Short).Show();
                            break;
                        }
                    }
                catch (Exception)
                {
                        Android.Widget.Toast.MakeText(this, "Server down", ToastLength.Short).Show();
                }
            }
        }
    }
}