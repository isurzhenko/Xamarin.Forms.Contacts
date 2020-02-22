# IvanSurzhenko.Xamarin.Forms.Contacts
Read Contacts Data on iOS and Android

<a href="https://www.nuget.org/packages/IvanSurzhenko.Xamarin.Forms.Contacts/">
<img src="https://img.shields.io/badge/Nuget-1.2.0-blue.svg">
</a>

Easy usage in Portable Project:

```csharp
var contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();
```


You can generate ObservableCollection like that:


```csharp
var contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync();

ObservableCollection<Contact> = new ObservableCollection<Contact>(contacts);

```


You can use filter like that:


```csharp
var contacts = await Plugin.ContactService.CrossContactService.Current.GetContactListAsync(x=>x.Emails.Count > 0);
```

# DEPENDENCIES
This package is depdended to Plugin.Permissions. Don't forget to add it and make the right setup
https://github.com/jamesmontemagno/PermissionsPlugin
      

# ANDROID
```
READ_CONTACTS
```

# iOS
If you don't have mac connection you should Right Click the **Info.plist** and Open With XML editor.
Add this key into **<dict>**
```xml
	<key>NSContactsUsageDescription</key>
	<string>We need contact permission to do ...</string>
```

