using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Android.Support.V4.App;
using Plugin.ContactService.Shared;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.ContactService
{
    /// <summary>
    ///     Interface for $safeprojectgroupname$
    /// </summary>
    public class ContactServiceImplementation : IContactService
    {
        public IEnumerable<Contact> GetContactList(Func<Contact, bool> filter = null)
        {
            var task = Task.Run(async () => await GetContacts(filter));
            task.Wait();
            return task.Result;
        }


        public Task<IEnumerable<Contact>> GetContactListAsync(Func<Contact, bool> filter = null) => GetContacts(filter);

        private async Task<IEnumerable<Contact>> GetContacts(Func<Contact, bool> filter = null)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Contacts);
                if (status != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Contacts);

                    var statuses = await CrossPermissions.Current.RequestPermissionsAsync(new Permission[] { Permission.Contacts });

                    foreach (var st in statuses)
                    {
                        if (st.Key == Permission.Contacts)
                        {
                            status = st.Value;
                        }
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    //Query permission
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }
            }
            catch (Exception ex)
            {
                //Something went wrong
            }
            var uri = ContactsContract.Contacts.ContentUri;

            var ctx = Application.Context;
            var cursor = ctx.ApplicationContext.ContentResolver.Query(uri, null, null, null, null);
            if (cursor.Count == 0) return new List<Contact>();

            List<Contact> result = new List<Contact>();

            while (cursor.MoveToNext())
            {
                var contact = CreateContact(cursor, ctx);

                if (filter != null && !filter(contact))
                    continue;

                if (!string.IsNullOrWhiteSpace(contact.Name))
                    result.Add(contact);
            }

            cursor.Close();

            return result;
        }

        private static Contact CreateContact(ICursor cursor, Context ctx)
        {
            var contactId = GetString(cursor, ContactsContract.Contacts.InterfaceConsts.Id);
            //            var hasNumbers = GetString(cursor, ContactsContract.Contacts.InterfaceConsts.HasPhoneNumber) == "1";

            var numbers = new List<string>(GetNumbers(ctx, contactId))
            var emails = new List<string>(GetEmails(ctx, contactId));

            var contact = new Contact
            {
                Name = GetString(cursor, ContactsContract.Contacts.InterfaceConsts.DisplayName),
                PhotoUri = GetString(cursor, ContactsContract.Contacts.InterfaceConsts.PhotoUri),
                PhotoUriThumbnail = GetString(cursor, ContactsContract.Contacts.InterfaceConsts.PhotoThumbnailUri),
                Emails = emails,
                Numbers = numbers,
            };

            return contact;
        }

        private static IEnumerable<string> GetNumbers(Context ctx, string contactId)
        {
            var key = ContactsContract.CommonDataKinds.Phone.Number;

            var cursor = ctx.ApplicationContext.ContentResolver.Query(
                ContactsContract.CommonDataKinds.Phone.ContentUri,
                null,
                ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?",
                new[] { contactId },
                null
            );

            return ReadCursorItems(cursor, key);
        }

        private static IEnumerable<string> GetEmails(Context ctx, string contactId)
        {
            var key = ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data;

            var cursor = ctx.ApplicationContext.ContentResolver.Query(
                ContactsContract.CommonDataKinds.Email.ContentUri,
                null,
                ContactsContract.CommonDataKinds.Email.InterfaceConsts.ContactId + " = ?",
                new[] { contactId },
                null);

            return ReadCursorItems(cursor, key);
        }

        private static IEnumerable<string> ReadCursorItems(ICursor cursor, string key)
        {
            while (cursor.MoveToNext())
            {
                var value = GetString(cursor, key);
                yield return value;
            }

            cursor.Close();
        }

        private static string GetString(ICursor cursor, string key)
        {
            return cursor.GetString(cursor.GetColumnIndex(key));
        }
    }
}
