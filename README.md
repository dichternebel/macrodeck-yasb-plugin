# Yet another Streamer.Bot Plugin
Yet another Streamer.Bot plugin for Macro Deck. 

## Installation
Go to the [releases](https://github.com/dichternebel/macrodeck-yasb-plugin/releases) and download the latest version.  
Open Macro Deck, go to the Plugins tab and click on the `Install from file` button.  
Configure the plugin to match your Streamer.Bot Websocket Server settings and you're good to go!

## Usage
The plugin will automatically connect to the Streamer.Bot Websocket Server and listen for events configured in the plugin settings.

## Configuration
It does **not** require any actions to be configured in Streamer.Bot. If you used the *'official'* plugin before, you should remove the action that sends the Global Variable `General.Custom` event or you will get duplicate events. Alternatively, if you want to keep the action you can also disable the `Misc.GlobalVariableUpdated` event in the plugin settings.

## Current state
The plugin is currently only tested with the following Streamer.Bot events:
- `General.Custom`
- `Misc.GlobalVariableUpdated`
- `Misc.UserGlobalVariableUpdated`
- `OBS.Connected`
- `OBS.Disconnected`

## Requirements
- Streamer.Bot 0.2.5 or higher with Websocket Server running and configured without any authentication
- Macro Deck 2.14.1 or higher
