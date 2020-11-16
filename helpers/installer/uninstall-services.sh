#!/usr/bin/env bash
function isServiceInstalled {
    grep=$(systemctl list-units --full -all | grep "$1")
    if [ $? -eq 0 ]; then
        return 0
    else
        return 1
    fi
}
if isServiceInstalled "ChristmasPi.service"; then
    echo "Uninstalling ChristmasPi.service"
    systemctl stop ChristmasPi.service
    find /etc/systemd/system/ -name ChristmasPi.service -exec rm -rf {} \;
    systemctl disable ChristmasPi.service
    systemctl daemon-reload
fi
if isServiceInstalled "Scheduler.service"; then
    echo "Uninstalling Scheduler.service"
    systemctl stop Scheduler.service
    find /etc/systemd/system/ -name Scheduler.service -exec rm -rf {} \;
    systemctl disable Scheduler.service
    systemctl daemon-reload
fi