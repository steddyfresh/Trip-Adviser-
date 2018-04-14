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

namespace Plan_My_Trip
{
    [Activity(Label = "reportActivity")]
    public class reportActivity : Activity
    {
       // StreamReader rfile;
        ListView lsreport;
        ArrayAdapter<string> adapter;
        Button close;
        Button logout;
        int id = 0;
        string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        string filename = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.report);
            // Create your application here
            id = Convert.ToInt32(Intent.GetStringExtra("text"));
            filename = Path.Combine(path, "report.data");
            Android.Widget.Toast.MakeText(this, "PMT Report", ToastLength.Long).Show();
            lsreport = FindViewById<ListView>(Resource.Id.listViewreport);
            List<string> triprep = new List<string>();
            close = FindViewById<Button>(Resource.Id.buttonClose);
            close.Click += Close_Click;
            logout = FindViewById<Button>(Resource.Id.buttonlogout);
            logout.Click += Logout_Click;
            try
            {
                Android.Widget.Toast.MakeText(this, "Loaded report file", ToastLength.Long).Show();
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/" + id.ToString() + "/users.pmt");
                if (stream != null)
                {
                    StreamReader reader = new StreamReader(stream);
                    while (!reader.EndOfStream)
                    {
                        string d = reader.ReadLine();
                        if (!d.Contains('<'))
                            triprep.Add(d);
                        else
                            break;
                    }
                    reader.Close();
                }
                stream.Close();
                if (triprep.Count > 0)
                    adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, triprep.ToArray());
                else
                {
                    adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
                    adapter.Add("No trip data found");
                }
                if (adapter.Count > 0)
                    lsreport.Adapter = adapter;

            } catch (Exception) {
                
            }
            
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            Android.Widget.Toast.MakeText(this, "Logging out...", ToastLength.Long).Show();
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Android.Widget.Toast.MakeText(this, "Closing...", ToastLength.Short).Show();
            var intent = new Intent(this, typeof(pmtActivity));
            intent.PutExtra("text", id.ToString());
            StartActivity(intent);
        }
    }
}