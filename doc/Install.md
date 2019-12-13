# Install


## Services

### ChristmasPi Service
sudo nano /etc/systemd/system/ChristmasPi.service
sudo chmod 664 /etc/systemd/system/ChristmasPi.service
sudo systemctl enable ChristmasPi.service

### Scheduler Service
sudo nano /etc/systemd/system/ChristmasPi-Scheduler.service
sudo chmod 664 /etc/systemd/system/ChristmasPi-Scheduler.service
sudo systemctl enable ChristmasPi-Scheduler.service