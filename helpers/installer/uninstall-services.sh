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
    rm /etc/systemd/system/ChristmasPi.service
    systemctl disable ChristmasPi.service
    systemctl daemon-reload
fi
if isServiceInstalled "Scheduler.service"; then
    echo "Uninstalling Scheduler.service"
    rm /etc/systemd/system/Scheduler.service
    systemctl disable Scheduler.service
    systemctl daemon-reload
fi