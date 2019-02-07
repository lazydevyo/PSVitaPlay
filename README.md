# PSVitaDock
A Streaming app which let's you plug and play your PSVita to your PC without hassle.


Actual knowing issues: Some times the audio gliches and it's in Mono channel if that bothers you instead of using the in app sound you can mute the app and enabled from your windows audio panel the ability to hear your audio input through the speakers.
And last if you connect an external audio card like usb headphones for example while playing the app may freeze and crash.

Preperation: 
The app takes advantage of xerpi's UDCD USB Video Class plugin, so make sure you have it installed on your PSVita.
Link for xerpi's plugin https://github.com/xerpi/vita_udcd_uvc. 
1. Download The PSVitaDock: https://github.com/lazydevyo/PSVitaDock/releases 
2. Connect your PSVita to mic-in or audio-in input on your computer. For the app to recognize which audio input is from your PSVita go to your Sound Settings then recording find which is your Vita for example "Microphone Line-In Front" or "Audio Line-In Rear" double click and rename it by adding the suffix or prefix "PSVita" so it should be like "Audio Line-In Rear PSVita".
Theres a big change your computer has Realtek audio interface so it doesn't matter if you connect it to mic input (Pink) or line-in input (Blue) just in case you connect it to the mic input make sure you pick the mic input to work as line in input from the Realtek panel beacuse mic input is to sensitive and the sound would be loud.
3. Start the PSVitaDock and Enjoy!

Shortcuts:
Enter(Return key): Shows up "Options menu".
ESC (Escape): Goes fullscreen or windowed.
TAB: Changes quality from HQ To smooth or vice-versa.
SPACE: Takes a screenshot.
In case you have any problem i added a PDF file with all the errors and solutions.

# To build your own

Download everything from the source.</br>

Download the AvProCamera unity package http://www.renderheads.com/downloads/2018/UnityPlugin-AVProLiveCamera-Latest-Trial.unitypackage</br>

Download the Post Processing Package https://assetstore.unity.com/packages/essentials/post-processing-stack-83912</br>

Install the unity packages.</br>

The PSVitaDock.unity scene is for the PC version app.</br>

The VitaDock.unity scene is for the Vita app "Remote PC". (To build the vitadock.unity for the vita you need the official psp2sdk for unity) and a licence for it</br>

Build each scene seperatly.
