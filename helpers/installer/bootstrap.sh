#!/usr/bin/env bash

# Constants
dotnet_installer="https://dot.net/v1/dotnet-install.sh"
dotnet_version="3.1"
dotnet_install_location="/home/$USER/.dotnet/"
christmaspi_release_url=""

# Arguments

function prompt_yes_no {
    while true; do
        read -p "$1 (y/n)" yn
        case $yn in
            [Yy]* ) return 0;;
            [Nn]* ) return 1;;
            * ) echo "Please answer yes or no.";;
        esac
    done
}

function install_prereqs {

}

function is_dotnet_installed {
    command -v dotnet &> /dev/null
    return $?
}

function install_dotnet {
    cd /tmp/
    if [ -f "dotnet-install.sh" ]; then
        rm "dotnet-install.sh"
    fi
    wget $dotnet_installer
    chmod +x dotnet-install.sh
    ./dotnet-install.sh -c $dotnet_version --install-dir $dotnet_install_location
    echo 'export PATH="$dotnet_install_location:$PATH"' >> /home/$USER/.bashrc
    source /home/$USER/.bashrc
}

function download_christmaspi {

}

function run_christmaspi {
    echo "ChristmasPi must be run as root, enter your password to run ChristmasPi:"
    read -s password
    echo "PASSWORD" | sudo -S dotnet run
}

function main() {
    install_prereqs
    if [ ! is_dotnet_installed ]; then
        install_dotnet
    fi
    if [ -d /usr/local/ChristmasPi ]; then
        echo "ChristmasPi directory already exists, proceeding with this installation will remove that directory."
        if [ ! prompt_yes_no "Do you wish to proceed?" ]; then
            echo "Exiting"
            exit 1
        fi
        rm -rf /usr/local/ChristmasPi/
    fi
    mkdir /usr/local/ChristmasPi
    download_christmaspi
}