$(document).ready(function () {
    "use strict";

    Core.init();

    var alert = $('#alert'),
        breakdown = $('#breakdown-widget'),
        tableMessages = $('#tableMessages'),
        inputStart = $('#inputStart'),
        inputEnd = $('#inputEnd'),
        buttonMessages = $('#formMessages').find('button'),
        end = moment().format('MM/DD/YYYY h:mm A'),
        start = moment(end).subtract(1, 'hours').format('MM/DD/YYYY h:mm A');

    tableMessages.footable();

    inputStart.val(start);
    inputEnd.val(end);

    inputStart.datetimepicker();
    inputEnd.datetimepicker();

    buttonMessages.click(function (event) {
        event.preventDefault();

        var from = inputStart.val(),
            to = inputEnd.val(),
            createRow = function(data) {
                return $(
                    '<tr><td><a href="/messages/' + data.id + '">' + data.id + '</a>' +
                    '</td><td>' + data.nick +
                    '</td><td>' + data.content +
                    '</td><td nowrap="nowrap">' + moment(data.created).format('MM/DD/YYYY h:mm A') +
                    '</td></tr>');
            };

        $.ajax({
            url: '/api/messages?from=' + from + '&to=' + to,
            success: function (data) {
                if (!data || data.length === 0) {
                    alert.fadeIn();
                    return;
                }

                alert.fadeOut();

                var tableBody = tableMessages.find('tbody'),
                    tableBoyRows = tableBody.find('tr');

                tableBoyRows.remove();

                $.each(data, function (index, item) {
                    tableBody.append(createRow(item));
                });

                tableBody.trigger('footable_redraw');

                //$('#breakdown-pie').highcharts({
                //    credits: false,
                //    chart: {
                //        plotBackgroundColor: null,
                //        plotBorderWidth: null,
                //        plotShadow: false
                //    },
                //    title: {
                //        text: null
                //    },
                //    tooltip: {
                //        pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                //    },
                //    plotOptions: {
                //        pie: {
                //            center: ['30%', '50%'],
                //            allowPointSelect: true,
                //            cursor: 'pointer',
                //            dataLabels: {
                //                enabled: false
                //            },
                //            showInLegend: true
                //        }
                //    },
                //    series: [{
                //        type: 'pie',
                //        name: 'Browser share',
                //        data: [
                //            ['Firefox', 35.0],
                //            ['IE', 36.8], {
                //                name: 'Chrome',
                //                y: 15.8,
                //                sliced: true,
                //                selected: true
                //            },
                //            ['Safari', 18.5]
                //        ]
                //    }]
                //});

                breakdown.fadeIn();
            }
        });
    });
});
