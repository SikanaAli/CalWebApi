



let nextRefreshEle = $("#next-refresh-time");

const InitElements = () => {
    nextRefreshEle.text("Next Refresh "+Date().split(' ')[4])
    $("#task-dataTable").DataTable({
        "ajax": '../api/v1/Scheduler',
        "columns": [
            { "data": "taskName" },
            { "data": "group" },
            { "data": "nextFireTime" },
            { "data": "previousFireTime" },
        ]
    } )
}

$(() => {
    InitElements()
})


//Task Data Table

