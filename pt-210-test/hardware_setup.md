# Hardware setup

Device is [GoojPrt PT-210](http://www.xmjprt.com/bbx/2457739-2457739.html?id=52829&pid=2466489).
It is ECS/POS compatible thermal printer.

## Linux

Tried on Raspberry Pi 3 Model B.

It's better to keep CUPS off (or uninstalled).

```bash
sudo service cups stop
```

### USB

Device is `/dev/usb/lp0`.

Use

``` bash
sudo chmod +777 /dev/usb/lp0
```

or use `sudo` to start application.

### Bluetooth

Use `bluetoothctl` to pair device (`scan`/`pair` commands).
Check if printer is listed and get MAC adddress.

```bash
bluetoothctl devices
bluetoothctl info PT-210
```

Use `rfcomm` to bind device to actual file dev.

```bash
sudo rfcomm bind /dev/rfcomm0 <MAC address> 1
```

Can test file device with `picocom`.

```bash
sudo picocom -c /dev/rfcomm0
```

Release device.

```bash
sudo rfcomm release /dev/rfcomm0 
```

## Windows

USB should work with [manufacturer driver](http://www.xmjprt.com/bbx/2457747-2457747.html).

Bluetooth should work with BT paired device. Check device properties to see what COM port it actually uses.

## CUPS

For CUPS there are [manufacturer linux driver](http://www.xmjprt.com/bbx/2457747-2457747.html).

In case of ARM arch there is compatible [Zijiang ZJ-58 custom driver](https://github.com/klirichek/zj-58).
For Windows client there are [Zijang driver](http://www.zjiang.com/en/init.php/service/driver) works as well. 
