## fKeys
# Send key inputs to different PCs on the network

1. Start fKeys - Receiver binary on the PC that you want to receive the key inputs.
2. Start fKeys - Sender on the PC which you want to send key inputs from.

# Configuring fKeys - Sender

- Set the target IP (the receiver on the other PC shows the current ip)
- Click in the white box and press the keybind combination that you want to use
- Click enable Hotkey (You can disable it any time)

You only have to configure it once, since the keybind and the target IP are stored in `keybinds.cfg` and `target.cfg` in the sender's folder. 
**Note** : Port 5005 is used by default, in order to change this, a rebuild is needed in this version.
