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
    [Activity(Label = "userdataActivity")]
    public class userdataActivity : Activity
    {
        EditText surname;
        EditText name;
        EditText contact;
        EditText kinsurname;
        EditText kinname;
        EditText kincontact;
        EditText password;
        Button save;
        int id = 0;
        string destination = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.userdata);
            // Create your application here
            Android.Widget.Toast.MakeText(this, "Registration", ToastLength.Long).Show();

            destination = Intent.GetStringExtra("text");

            save = FindViewById<Button>(Resource.Id.buttonsave);
            save.Click += Save_Click;

            surname = FindViewById<EditText>(Resource.Id.editTextsurname);
            surname.Text = "";

            name = FindViewById<EditText>(Resource.Id.editTextName);
            name.Text = "";

            contact = FindViewById<EditText>(Resource.Id.editTextcon);
            contact.Text = "";

            kinsurname = FindViewById<EditText>(Resource.Id.editTextksurname);
            kinsurname.Text = "";

            kinname = FindViewById<EditText>(Resource.Id.editTextkname);
            kinname.Text = "";

            kincontact = FindViewById<EditText>(Resource.Id.editTextkcon);
            kincontact.Text = "";

            password = FindViewById<EditText>(Resource.Id.editTextpass);
            password.Text = "";
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                string response = new WebClient().DownloadString("http://www.planmytrip.net23.net/id.php");
                try {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/confirm.pmt");
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadLine().Trim();
                    stream.Close();
                    reader.Close();
                    switch (content.ToLower())
                    {
                        case "yes":
                            try {
                                stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/id.pmt");
                                reader = new StreamReader(stream);
                                content = reader.ReadLine().Trim();
                                stream.Close();
                                reader.Close();
                                if (content.Length > 0)
                                    id = Convert.ToInt32(content);
                                else
                                    Android.Widget.Toast.MakeText(this, "Error: Cannot assign id to user", ToastLength.Short).Show();
                            } catch (Exception) {
                                Android.Widget.Toast.MakeText(this, "Error: Cannot find id file on server", ToastLength.Short).Show();
                            }
                            
                            break;
                        case "no":
                            Android.Widget.Toast.MakeText(this, "Error: No data on server", ToastLength.Long).Show();
                            break;
                    }

                    } catch (Exception err) {
                    //Android.Widget.Toast.MakeText(this, "Error: Cannot find confirm id file on server", ToastLength.Long).Show();
                    Android.Widget.Toast.MakeText(this, "Error: " + err.ToString(), ToastLength.Long).Show();
                }
            }
            catch (Exception err)
            {
                //Android.Widget.Toast.MakeText(this, "Server down", ToastLength.Long).Show();
                Android.Widget.Toast.MakeText(this, "Error: " + err.ToString(), ToastLength.Long).Show();
            }
            if(id > 0)
            {
                if (surname.Text != " " && name.Text != " " && contact.Text != " " && kinsurname.Text != " " && kinname.Text != " " && kincontact.Text != " " && password.Text != " ")
                {

                    try
                    {
                        string response = new WebClient().DownloadString("http://www.planmytrip.net23.net/insert.php?us=" + surname.Text + "&un=" + name.Text
                            + "&uc=" + contact.Text + "&up=" + password.Text + "&ks=" + kinsurname.Text + "&kn=" + kinname.Text + "&kc=" + kincontact.Text + "&id=" + id.ToString() + "&ud=" + destination+"");
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/insert.pmt");
                        StreamReader reader = new StreamReader(stream);
                        string content = reader.ReadLine().Trim();
                        stream.Close();
                        reader.Close();
                        var intent = new Intent();
                        switch (content.ToLower())
                        {
                            case "yes":
                                Android.Widget.Toast.MakeText(this, "User's data saved on database", ToastLength.Short).Show();
                                Android.Widget.Toast.MakeText(this, "Username is your name,and password is your password", ToastLength.Long).Show();
                                intent = new Intent(this, typeof(pmtActivity));
                                intent.PutExtra("text", id.ToString());
                                StartActivity(intent);
                                break;
                            case "up":
                                Android.Widget.Toast.MakeText(this, "Updated user's data", ToastLength.Short).Show();
                                try
                                {
                                    stream = client.OpenRead("http://www.planmytrip.net23.net/DATA/id.pmt");
                                    reader = new StreamReader(stream);
                                    content = reader.ReadLine().Trim();
                                    stream.Close();
                                    reader.Close();
                                    if (content.Length > 0)
                                    {
                                        id = Convert.ToInt32(content);
                                        intent = new Intent(this, typeof(pmtActivity));
                                        intent.PutExtra("text", id.ToString());
                                        StartActivity(intent);
                                        Android.Widget.Toast.MakeText(this, "Updated user id", ToastLength.Short).Show();
                                    }
                                    else
                                    {
                                        Android.Widget.Toast.MakeText(this, "Error: Cannot update user id", ToastLength.Short).Show();
                                    }
                                }
                                catch (Exception)
                                {
                                    Android.Widget.Toast.MakeText(this, "Error: server down", ToastLength.Short).Show();
                                }
                                break;
                            case "no":
                                Android.Widget.Toast.MakeText(this, "Error: User's data not uploaded", ToastLength.Short).Show();
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Android.Widget.Toast.MakeText(this, "Error: Server is down,retry", ToastLength.Short).Show();
                    }
                }
                else
                {
                    Android.Widget.Toast.MakeText(this, "Error: Please fill all fields", ToastLength.Short).Show();
                }
            }
            else
            {
                Android.Widget.Toast.MakeText(this, "Error: user id on server", ToastLength.Short).Show();
            }
        }
    }
}