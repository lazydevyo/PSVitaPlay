# PSVitaDock
An Streaming app which let's you plug and play your PSVita to your PC without hustle.


Actual knowing issues: Theres a big change you gonna have audio latency and theres a small change you won't but most of the time theres an audio latency about 0.2 ms to 0.5ms, For now instead of using the in app sound you can mute the app and enabled from your windows audio panel the ability to hear your audio input through the speakers.
The plugin i used to stream the PSVita has a watermark as seen in the previous post video basicaly the green purple line.
The fix for that is either finding a new plugin to embed in the app or either buying the plugin which is about 180 Euros :(.
And last if you connect an external audio card like usb headphones for example while playing the app may freeze and crash.

Preperation: 
The app takes advantage of xerpi's UDCD USB Video Class plugin, so make sure you have it installed on your PSVita.
Link for xerpi's plugin https://github.com/xerpi/vita_udcd_uvc. 
1. Download The PSVitaDock: 
2. Connect your PSVita to mic-in or audio-in input on your computer. For the app to recognize which audio input is from your PSVita go to your Sound Settings then recording find which is your Vita for example "Microphone Line-In Front" or "Audio Line-In Rear" double click and rename it by adding the suffix or prefix "PSVita" so it should be like "Audio Line-In Rear PSVita".
Theres a big change your computer has Realtek audio interface so it doesn't matter if you connect it to mic input (Pink) or line-in input (Blue) just in case you connect it to the mic input make sure you pick the mic input to work as line in input from the Realtek panel beacuse mic input is to sensitive and the sound would be loud.
3. Start the PSVitaDock and Enjoy!
