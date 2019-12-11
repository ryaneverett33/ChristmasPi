// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var currentColor;

function setActivePage(active) {
    switch (active) {
        case "Home":
            if ($('#li-nav-solid').hasClass('active'))
                $('#li-nav-solid').removeClass('active');
            if ($('#li-nav-animation').hasClass('active'))
                $('#li-nav-animation').removeClass('active');
            $('#li-nav-home').addClass('active');
            break;
        case "Animation":
            if ($('#li-nav-solid').hasClass('active'))
                $('#li-nav-solid').removeClass('active');
            if ($('#li-nav-home').hasClass('active'))
                $('#li-nav-home').removeClass('active');
            $('#li-nav-animation').addClass('active');
            break;
        case "Solid":
            if ($('#li-nav-animation').hasClass('active'))
                $('#li-nav-animation').removeClass('active');
            if ($('#li-nav-home').hasClass('active'))
                $('#li-nav-home').removeClass('active');
            $('#li-nav-solid').addClass('active');
            break;
    }
}
$(function () {
    $('#cp2').colorpicker({
        inline: true,
        container: false,
        format: "rgb",
        useAlpha: false,
        customClass: 'colorpicker-2x',
        sliders: {
            saturation: {
                maxLeft: 200,
                maxTop: 200
            },
            hue: {
                maxTop: 200
            },
            alpha: {
                maxTop: 200
            }
        }
    });
    $('#cp2').on('colorpickerChange colorpickerCreate', function (event) {
        currentColor = event.color.toString();
    });
    $("#power-checkbox").bootstrapSwitch();
    $('#power-checkbox').on('switchChange.bootstrapSwitch', function (e, data) {
        if (data)
            TurnOn();
        else
            TurnOff();
    });
});
function showErrorModal(errorMessage) {
    $("#genericErrorModalMessage").text(errorMessage);
    $('#genericErrorModal').modal();
}
function ShowAddRuleModal() {
    $("#addrule-starttime").datetimepicker({
        format: 'LT'
    });
    $("#addrule-endtime").datetimepicker({
        format: 'LT'
    });
    $('#ScheduleAddModal').modal();
}
function ShowRemoveRuleModal() {
    $('#ScheduleRemoveModal').modal();
}

function SetColor(colorstring) {
    $("#cp2").colorpicker('setValue', colorstring);
}

function SubmitColor() {
    // get color
    var color = currentColor;
    console.log(color);
    // /api/solid/update?color=db7b26
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to set color");
        }
        else {
            location.reload();
        }
    });
    //oReq.open("POST", "/api/solid/update?color=" + color);
    oReq.open("POST", "/api/solid/update");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({ color: color }));
}
function PlayAnimation(animation) {
    // /api/animations/play?animation=Twinkle
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to play animation");
        }
        else {
            location.reload();
        }
    });
    // oReq.open("POST", "/api/animations/play?animation=" + animation);
    oReq.open("POST", "/api/animations/play");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({ animation: animation }));
}
function ResumeAnimation() {
    // /api/animations/play
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to play animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/play");
    oReq.send();
}
function PauseAnimation() {
    // /api/animations/pause
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to pause animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/pause");
    oReq.send();
}
function StopAnimation() {
    // /api/animations/stop
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to stop animation");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/animations/stop");
    oReq.send();
}
function TurnOn() {
    // /api/power/on
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to turn on");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/power/on");
    oReq.send();
}
function TurnOff() {
    // /api/power/on
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to turn on");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/power/off");
    oReq.send();
}
// converts the checkbox values to a bitmask
function checkListToBitmask(monday, tuesday, wednesday, thursday, friday, saturday, sunday) {

}

function RemoveRule() {

}
function AddRule() {

}