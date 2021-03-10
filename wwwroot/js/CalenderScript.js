
function selectDate(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    console.log("Single Click");
}

function onDoubleClick(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    $('#calendar-modal').iziModal('open');
    console.log("Doublick done");
}

var defaultConfig = {
    weekDayLength: 1,
    date: new Date(),
    onClickDate: selectDate,
    onDoubleClickDate: onDoubleClick,
    showYearDropdown: true,
};

$('#calendar').calendar(defaultConfig);
$('.day').addClass('noselect');


//Calender Modal
$("#calendar-modal").iziModal({
    padding: "2rem",
    radius:5,
});