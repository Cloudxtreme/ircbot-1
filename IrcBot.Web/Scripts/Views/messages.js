$(document).ready(function () {
    "use strict";

    Core.init();

    var inputStart = $('#inputStart'),
        inputEnd = $('#inputEnd'),
        buttonMessages = $('#formMessages').find('button'),
        start = moment().startOf('day').format('MM/DD/YYYY h:mm A'),
        end = moment().endOf('day').format('MM/DD/YYYY h:mm A');

    inputStart.val(start);
    inputEnd.val(end);

    inputStart.datetimepicker();
    inputEnd.datetimepicker();

    buttonMessages.click(function (event) {
        event.preventDefault();

        var from = inputStart.val(),
            to = inputEnd.val(),
            table = $('.table tbody'),
            createRow = function (row) {
                return '<tr><td>' + row.nick + '</td><td>' + '<a href="/messages/' + row.id + '">' + row.content + '</a></td><td nowrap>' + moment(row.created).format('MMMM Do YYYY, h:mm:ss a') + '</td></tr>';
            };

        $.ajax({
            url: '/api/messages?from=' + from + '&to=' + to,
            success: function (result) {
                $.each(result, function (index, value) {
                    table.append(createRow(value));
                });
            },
            dataType: "json"
        });
    });
});
