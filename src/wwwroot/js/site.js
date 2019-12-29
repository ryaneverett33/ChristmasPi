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
        format: 'LT',
        icons: {
            time: "fa fa-clock-o",
            date: "fa fa-calendar",
            up: "fa fa-chevron-up",
            down: "fa fa-chevron-down",
            previous: 'fa fa-chevron-left',
            next: 'fa fa-chevron-right',
            today: 'fa fa-screenshot',
            clear: 'fa fa-trash',
            close: 'fa fa-remove'
        }
    });
    $('#ScheduleAddModal').modal();
}
function closeAddRuleModal() {
    $('#ScheduleAddModal').modal('hide');
}

function ShowRemoveRuleModal(i, j) {
    $('#scheduleremove-i').val(i);
    $('#scheduleremove-j').val(j);
    $('#ScheduleRemoveModal').modal();
}
function closeRemoveRuleModal() {
    $('#scheduleremove-i').val("");
    $('#scheduleremove-j').val("");
    $('#ScheduleRemoveModal').modal('hide');
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

function checkAddRuleDays() {
    var monday = $("#monday-check:checked").val() === "on";
    var tuesday = $("#tuesday-check:checked").val() === "on";
    var wednesday = $("#wednesday-check:checked").val() === "on";
    var thursday = $("#thursday-check:checked").val() === "on";
    var friday = $("#friday-check:checked").val() === "on";
    var saturday = $("#saturday-check:checked").val() === "on";
    var sunday = $("#sunday-check:checked").val() === "on";
    if (monday || tuesday || wednesday || thursday || friday || saturday || sunday)
        $("#scheduleAddRuleBtn").prop('disabled', false);
    else
        $("#scheduleAddRuleBtn").prop('disabled', true);
}

// seealso ChristmasPi.Data.Models.Scheduler.RepeatUsage
var REPEATMONDAY = 1;
var REPEATTUESDAY = 2;
var REPEATWEDNESDAY = 4;
var REPEATTHURSDAY = 8;
var REPEATFRIDAY = 16;
var REPEATSATURDAY = 32;
var REPEATSUNDAY = 64;

function getRepeatUsage() {
    var monday = $("#monday-check:checked").val() === "on";
    var tuesday = $("#tuesday-check:checked").val() === "on";
    var wednesday = $("#wednesday-check:checked").val() === "on";
    var thursday = $("#thursday-check:checked").val() === "on";
    var friday = $("#friday-check:checked").val() === "on";
    var saturday = $("#saturday-check:checked").val() === "on";
    var sunday = $("#sunday-check:checked").val() === "on";

    var mask = 0;
    if (monday)
        mask = mask | REPEATMONDAY;
    if (tuesday)
        mask = mask | REPEATTUESDAY;
    if (wednesday)
        mask = mask | REPEATWEDNESDAY;
    if (thursday)
        mask = mask | REPEATTHURSDAY;
    if (friday)
        mask = mask | REPEATFRIDAY;
    if (saturday)
        mask = mask | REPEATSATURDAY;
    if (sunday)
        mask = mask | REPEATSUNDAY;

    return mask;
}

function RemoveRule() {
    // /api/schedule/remove
    var i = parseInt($("#scheduleremove-i").val());
    var j = parseInt($("#scheduleremove-j").val());
    closeRemoveRuleModal();
    //var start = $(`#rstart-${i}:${j}`).val();
    var start = document.getElementById(`rstart-${i}:${j}`).value;
    //var end = $(`#rend-${i}:${j}`).val();
    var end = document.getElementById(`rend-${i}:${j}`).value;
    //var day = $(`#rday-${i}:${j}`).val();
    var day = document.getElementById(`rday-${i}:${j}`).value;

    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to remove rule");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/schedule/remove");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({
        start: start,
        end: end,
        day: day
    }));
}

function AddRule() {
    // /api/schedule/add
    var start = $('#addrule-starttime').data("DateTimePicker").viewDate().format("HH:mm");
    var end = $('#addrule-endtime').data("DateTimePicker").viewDate().format("HH:mm");
    var repeat = getRepeatUsage();
    closeAddRuleModal();

    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to add rule");
        }
        else {
            location.reload();
        }
    });
    oReq.open("POST", "/api/schedule/add");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send(JSON.stringify({
        start: start,
        end: end,
        repeat: repeat
    }));
}

function StartSetup() {
    var oReq = new XMLHttpRequest();
    oReq.addEventListener("load", function () {
        if (this.status !== 200) {
            showErrorModal("Failed to add rule");
        }
        else {
            location.href="/setup/next"
        }
    });
    oReq.open("POST", "/setup/start");
    oReq.setRequestHeader("Content-Type", "application/json");
    oReq.send();
}