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
    [Activity(Label = "registerActivity")]
    public class registerActivity : Activity
    {
        Button search;
        EditText edtsearch;
        ListView lstsearch;
        ArrayAdapter<string> adapter;
        string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register);
            // Create your application here
            Android.Widget.Toast.MakeText(this, "Please search your destination", ToastLength.Long).Show();
            search = FindViewById<Button>(Resource.Id.buttonsearch);
            search.Click += Search_Click;
            edtsearch = FindViewById<EditText>(Resource.Id.editTextsearch);
            edtsearch.Text = "";
            lstsearch = FindViewById<ListView>(Resource.Id.listViewresult);
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
            lstsearch.Adapter = adapter;
            lstsearch.Visibility = ViewStates.Gone;
           
        }
        private void Search_Click(object sender, EventArgs e)
        {
            adapter.Clear();
            lstsearch.Visibility = ViewStates.Visible;
            try
            {
                if (edtsearch.Text.Length > 0)
                {
                    string response = new WebClient().DownloadString("http://www.planmytrip.net23.net/search.php?ds=" + edtsearch.Text.Trim());
                    try {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/search.pmt");
                        int counting = 0;
                        String content = "";
                        if (stream != null)
                        {
                            StreamReader reader = new StreamReader(stream);      
                            while (!reader.EndOfStream)
                            {
                                content = reader.ReadLine();
                                adapter.Add(content);
                                adapter.NotifyDataSetChanged();
                                counting++;
                            }        
                            reader.Close();
                        }
                        stream.Close();

                        if (counting == 0)
                            adapter.Add("Destination does not exist");
                        else if(counting > 1)
                            adapter.Add("Please search exact destination");
                        if(adapter.Count > 0)
                            lstsearch.Adapter = adapter;
                        if(counting == 1 && content != "")
                        {
                            var intent = new Intent(this, typeof(userdataActivity));
                            intent.PutExtra("text", content);
                            StartActivity(intent);
                        }
                           
                    }
                    catch (Exception) {
                        Android.Widget.Toast.MakeText(this, "Error: Cannot find destination file", ToastLength.Short).Show();
                    }
                }
                else
                {
                    Android.Widget.Toast.MakeText(this, "Error: Please fill the field", ToastLength.Short).Show();
                }
               
            }
            catch (Exception) {
                Android.Widget.Toast.MakeText(this, "Error: Server down", ToastLength.Short).Show();
            }
        }
    }
}