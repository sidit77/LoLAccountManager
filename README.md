# LoL Account Manager
I got tired of having a `lolaccounts.txt` on my Desktop and manually copy and pasting passwords.

## Demo

![](https://thumbs.gfycat.com/SkeletalRespectfulCavy-size_restricted.gif)

## Security

The usernames and passwords are stored in a file called `accounts.ladb` next to the executable. This file is encrypted using the Windows password of the current user and never leaves the current pc. It's probably a good idea to keep in mind that this isn't meant to be a secure password manager but a more comfortable replacement for a plain text file. There is no master password and I have no experience writing secure software. If you want to confirm for yourself that this program doesn't send your password to a secret server feel free to look at the source code for the [Manager](https://github.com/sidit77/LoLAccountManager/blob/master/LoLPasswordManager/Form1.cs) (Loading, Saving, UI) and the [Autofill helper](https://github.com/sidit77/LoLAccountManager/blob/master/LoLPasswordManager/LoginHelper.cs).

## Download

[Github release page](https://github.com/sidit77/LoLAccountManager/releases)

## Known Problems

This programs autofill works by emulating keypresses using the win32 api. Sometimes the program fails to properly focus the Riot Client and therefore ends up clicking the *can't sign in* link instead of the login button. If that happens just try again. 