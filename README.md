# Yet another Streamer.Bot Plugin
![Streamer.Bot logo](Resources/streamerbot-logo-white.png) Yet another Streamer.Bot plugin for Macro Deck.

## Manual Installation
Go to the [releases](https://github.com/dichternebel/macrodeck-yasb-plugin/releases) and download the latest version.  
Open Macro Deck, go to the Plugins tab and click on the `Install from file` button.  

By default the plugin listens to `General.Custom` and `Misc.GlobalVariableUpdated` events. Configure the plugin to match your Streamer.Bot websocket server settings and you're good to go!

## Usage
The plugin will automatically connect to the Streamer.Bot websocket server and listen for events configured in the plugin settings.

## Configuration in Streamer.Bot

The Streamer.Bot websocket server has to run.

This plugin does **not** require any actions to be configured in Streamer.Bot. If you used the *'official'* plugin before, you should remove the action that sends the Global Variable `General.Custom` event or you will get duplicate events. Alternatively, if you want to keep the action you can also disable the `Misc.GlobalVariableUpdated` event in the plugin settings.

## Events
You can configure the plugin to listen to all available events in Streamer.Bot.
Events that are not selected in the plugin events setting will be ignored.

## Variables
The plugin will transform following events to display their actual value in Macro Deck variables:
- `General.Custom`
- `Misc.GlobalVariableUpdated`

Other events will create variable values in `JSON` format.  
Variable keys are stored in the format `groupname_variablename`:

![Variables Screenshot](Resources/yasb-variables-screenshot.png)

## Actions
The plugin comes with a Macro Deck action called `DoAction` that can be used to execute any existing action in Streamer.Bot:

![DoAction Screenshot](Resources/yasb-doaction-screenshot.png)

To configure the Macro Deck action, you need to enter the `Action ID` and the `Action Name` of the action you want to trigger or you can select the group of the action in the first combobox and then select the action in the second combobox and click on `Use`.

The `Argument` field is optional and can be used to pass an argument to the action. It has to be in valid `JSON` format like `{ "value": 42 }` or like seen in the screenshot above.

## Requirements
- Streamer.Bot 0.2.5 or higher with Websocket Server running and configured without any authentication
- Macro Deck 2.14.1 or higher
