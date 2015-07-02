$(document).ready(function () {
    "use strict";

    Core.init();
});

//require(['jquery', 'bootstrap', 'moment'], function ($, b, moment) {

//    $(function () {

//        var today = moment().format('YYYY-MM-DD'),
//            yesterday = moment().add(-1, 'days').format('YYYY-MM-DD');

//        $('#inputFrom').val(yesterday);
//        $('#inputTo').val(today);

//        $('#showMessages').click(function (e) {
//            e.preventDefault();

//            var from = $('#inputFrom').val(),
//                to = $('#inputTo').val(),
//                table = $('.table tbody'),
//                createRow = function (row) {
//                    return '<tr><td>' + row.nick + '</td><td>' + '<a href="/messages/' + row.id + '">' + row.content + '</a></td><td nowrap>' + moment(row.created).format('MMMM Do YYYY, h:mm:ss a') + '</td></tr>';
//                };

//            $.ajax({
//                url: '/api/messages?from=' + from + '&to=' + to,
//                success: function (result) {
//                    $.each(result, function (index, value) {
//                        table.append(createRow(value));
//                    });
//                },
//                dataType: "json"
//            });
//        });

        
//    });
//});