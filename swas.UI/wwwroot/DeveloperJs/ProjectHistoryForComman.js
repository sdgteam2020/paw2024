function GetProjectMovHistory(ProjId) {
    var listitem = "";

    $.ajax({
        url: '/Projects/ProjectMovHistory',
        type: 'POST',
        data: { "ProjectId": ProjId },
        success: function (response) {
            console.log(response);
            if (response.length) {
                listitem += '<div class="timeline-month">';
                listitem += '  ' + DateFormatedd_mm_yyyy(new Date($.now())) + '';
                listitem += '<span>' + response.length + ' Entries</span>';
                listitem += '</div>';
                for (var i = 0; i < response.length; i++) {
                    listitem += '<div class="timeline-section"> <div class="timeline-date"> ' + DateFormatedd_mm_yyyy(response[i].date) + '</div>';
                    listitem += '<div class="row"><div class="col-sm-4">';
                    listitem += '<div class="timeline-box">';
                    if (response[i].isComment == false) {
                        if (response[i].actions == "FWD" && (response[i].undoRemarks == "" || response[i].undoRemarks == null))
                            listitem += '<div class="box-title bg-warning  text-white"><i class="fa-solid fa-forward" style="color: #FFD43B;"></i> ' + response[i].actions + '</div>';
                        else if (response[i].undoRemarks == "" || response[i].undoRemarks == null)
                            listitem += '<div class="box-title bg-success text-white"><i class="fa-solid fa-circle-check fa-xl" style="color: #3adb00;"></i> ' + response[i].actions + '</div>';
                        else
                            listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-rotate-left fa-xl" style="color: #ffff;"></i> ' + response[i].actions + '</div>';

                    }
                    else if (response[i].isComment == true) {
                        listitem += '<div class="box-title bg-danger text-white"><i class="fa-solid fa-comments fa-xl" style="color: #ffff;"></i> For Comments</div>';

                    }
                    listitem += '<div class="box-content">';
                    /*listitem += '<a class="btn btn-xs btn-default pull-right">Details</a>';*/
                    listitem += '<div class="box-item"><strong>Stages</strong>: <span class="badge rounded-pill bg-primary">' + response[i].stages + '</span></div>';
                    listitem += '<div class="box-item"><strong>Sub Stages</strong>: <span class="badge rounded-pill bg-warning text-dark">' + response[i].status + '</span></div>';
                    listitem += '<div class="box-item"><strong>From Unit</strong>: <span class="badge rounded-pill bg-secondary">' + response[i].fromUnitName + '</span></div>';
                    listitem += '<div class="box-item"><strong>TO Unit</strong>: <span class="badge rounded-pill bg-info text-dark">' + response[i].toUnitName + '</span></div>';
                    listitem += '</div>';
                    listitem += '<div class="box-footer">' + response[i].userDetails + '</div>';
                    listitem += '</div> </div>';
                    if (response[i].remarks != "") {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i> Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + response[i].remarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + response[i].userDetails + '</div>';
                        listitem += '</div></div>';
                    }
                    if (response[i].undoRemarks != null) {
                        listitem += '<div class="col-sm-4">';
                        listitem += '<div class="timeline-box">';
                        listitem += '<div class="box-title">';
                        listitem += '<i class="fa fa-pencil text-info" aria-hidden="true"></i>Undo Remarks';
                        listitem += '</div>';
                        listitem += '<div class="box-content">';
                        /*listitem += ' <a class="btn btn-xs btn-default pull-right">Remarks</a>';*/
                        listitem += '<div class="box-item">' + response[i].undoRemarks + '</div>';
                        listitem += '</div>';
                        listitem += '<div class="box-footer">' + response[i].userDetails + '</div>';
                        listitem += '</div></div>';
                    }
                    listitem += '</div></div>';
                    listitem += '';
                    listitem += '';
                }


                $("#projectmovfistory").html(listitem);
            }
        }
    });
}