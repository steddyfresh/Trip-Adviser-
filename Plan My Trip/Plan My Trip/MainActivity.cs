using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.IO;
namespace Plan_My_Trip
{
    [Activity(Label = "Plan_My_Trip", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        CheckBox checkbox;
        Button button;
        EditText username;
        EditText password;
        TextView u;
        TextView p;
        
        //string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
       // string filename = "";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);   
            Android.Widget.Toast.MakeText(this, "Welcome to PMT", ToastLength.Long).Show();
            button = FindViewById<Button>(Resource.Id.buttonlogin);
            button.Click += Button_Click;
            checkbox = FindViewById<CheckBox>(Resource.Id.checkBoxnewuser);
            checkbox.CheckedChange += Checkbox_CheckedChange;
            username = FindViewById<EditText>(Resource.Id.editTextusername);
            username.Text = "";
            password = FindViewById<EditText>(Resource.Id.editTextpass);
            password.Text = "";
            u = FindViewById<TextView>(Resource.Id.textView2u);
            p = FindViewById<TextView>(Resource.Id.textView4p);     
        }

        private void Checkbox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (checkbox.Checked)
            {
                button.Text = "Register";
                visible(ViewStates.Gone);
            }
            else
            {
                button.Text = "Login";
                visible(ViewStates.Visible);
            }
        }
        void visible(ViewStates vs)
        {
            username.Visibility = vs;
            password.Visibility = vs;
            u.Visibility = vs;
            p.Visibility = vs;

        }
        private void Button_Click(object sender, EventArgs e)
        {
            var intent = new Intent();
            if (checkbox.Checked)
            {
                intent = new Intent(this, typeof(registerActivity));
                StartActivity(intent);
            }
            else
            {
                try
                {
                    if (username.Text.Length > 0 && password.Text.Length > 0)
                    {
                        string response = new WebClient().DownloadString("http://www.planmytrip.net23.net/login.php?un=" + username.Text + "&up=" + password.Text);
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead("http://www.planmytrip.net23.net/FOUND/found.pmt");
                        StreamReader reader = new StreamReader(stream);
                        String content = reader.ReadToEnd();
                        switch (content.ToLower())
                        {
                            case "yes":
                                stream = client.OpenRead("http://www.planmytrip.net23.net/FOUND/rem.pmt");
                                reader = new StreamReader(stream);
                                content = reader.ReadToEnd();
                                intent = new Intent(this, typeof(pmtActivity));
                                intent.PutExtra("text", content);
                                StartActivity(intent);
                                Android.Widget.Toast.MakeText(this, "Successful login", ToastLength.Short).Show();
                                break;
                            case "no":
                                Android.Widget.Toast.MakeText(this, "Error: User does not exist", ToastLength.Short).Show();
                                break;
                        }

                    }
                    else
                    {
                        Android.Widget.Toast.MakeText(this, "Error: Please fill all the fields", ToastLength.Short).Show();
                    }       
                } catch (Exception err) {
                    Android.Widget.Toast.MakeText(this, "Error: "+err.ToString(), ToastLength.Short).Show();
                }
                
            }        
        }
    }
}

