$(document).ready(function () {
    var editObj;
    var originalGraduatesData;

    $('.search-clear').popover({
        trigger: "hover",
        delay: { "hide": 200 },
        placement: "bottom"
    });

    var graduatesTable = $("#graduates-listing").DataTable({
        "pageLength": 50
    });
    originalGraduatesData = $("#graduates-listing").DataTable().data().toArray();

    var vacancyTable = $("#vacancy-listing").DataTable({
        "pageLength": 50,
        "columns": [
            { "width": "12%" },
            null,
            { "width": "23%" },
            null,
            null,
            { "width": "11%" },
            null,
            null,
            null
        ]
    });

    $(".search-clear").click(function () {
        $("#graduates-search-input").val("");
        $("#graduates-listing").DataTable().clear().draw();
        $("#graduates-listing").DataTable().rows.add(originalGraduatesData).draw();
        $(this).hide();
    });

    $("#graduates-search-input").marcoPolo({
        url: "/employer/seekers-search",
        delay: 50,
        cache: false,
        required: false,
        formatItem: function (data, $item) {
            return data.fullname;
        },
        formatData: function (data) {
            console.log(originalGraduatesData)
            var searchData = [];
            $.each(data, function (key, value) {
                var rowData = [value.firstName, value.middleName, value.lastName, value.email, value.jobTypesList, value.locationsList, "--"];
                searchData.push(rowData);
            });

            if ($("#graduates-search-input").length > 0) {
                $("#graduates-listing").DataTable().clear().draw();
                $("#graduates-listing").DataTable().rows.add(searchData).draw();
            }
            else
            {
                $("#graduates-listing").DataTable().clear().draw();
                $("#graduates-listing").DataTable().rows.add(originalGraduatesData).draw();
            }
            return data;
        },
        onChange: function (e) {
            if ($("#graduates-search-input").val().length === 0) {
                $("#graduates-listing").DataTable().clear().draw();
                $("#graduates-listing").DataTable().rows.add(originalGraduatesData).draw();
                $(".search-clear").hide();
            }
            else
            {
                $(".search-clear").show();
                $('#graduates-search-input').marcoPolo('search');
            }
        },
        onSelect: function (data, $item) {
            $("#graduates-search-input").val(data.fullname);
            $('#graduates-search-input').marcoPolo('search', data.fullname);
        },
        formatError: function ($item, jqXHR, textStatus, errorThrown) {
            console.log(errorThrown);
        },
        onNoResults: function (i, $item) {
            no_result = true;
        }
    });

    $("#btn-create-jpost").click(function () {
        window.location.href = "/employer/jobmanagement?intent=create";
    });

                                                                    
    $("#btn-create-jpost2").click(function () {
        $("#create-vacancy-modal").modal("show");
    });

    $("#btn-close-vacancy-modal").click(function () {
        ClearJobCreateFields();
    });

    $("#btn-post-vacancy").click(function () {
        var jobPostName = $("#vacancy-job-post-name").val();
        var jobType = $("#job-selector option:selected").val();
        var _jobLocations = $("#locations-selector option:selected");
        var startDate = $("#start-date").attr("data-dateraw");
        var endDate = $("#end-date").val();

        var jobLocations = [];
        $(_jobLocations).each(function () {
            var location = {};
            location.locationId = $(this).val();
            location.locationName = $(this).text();
            jobLocations.push(location);
        });

        if (_jobLocations.length > 0) {
            if (jobType !== "0") {
                var json = JSON.stringify({ "jobType": jobType, "jobTitle": jobPostName, "startDate": startDate, "endDate": endDate, "locations": jobLocations });
                $.ajax({
                    url: "/employer/add-vacancy",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: json,
                    success: function (result) {
                        if (result.status === "vacancy_created") {
                            var row = vacancyTable.row.add([
                                result.vacancy.jobTitle,
                                result.vacancy.html_jobType,
                                result.vacancy.html_locations,
                                result.vacancy.html_startDate,
                                result.vacancy.html_endDate,
                                result.vacancy.html_vacancyStatus,
                                "--",
                                "<i data-vid='" + result.vacancy.employerVacancyId + "' class='fas fa-edit btn-edit'></i>",
                                "<i data-vid='" + result.vacancy.employerVacancyId + "' class='fas fa-trash-alt btn-delete'></i>"
                            ]).draw(false).node();
                            console.log(result);
                            console.log(row);

                            setTimeout(function () {
                                $(row).addClass("row-highlight").delay(5000).queue(function (next) {
                                    $(this).removeClass("row-highlight");
                                    next();
                                });
                            }, 200);

                            //alert("Job vacancy: " + result.vacancy.jobTitle + " has been created successfuly");
                            $("#create-vacancy-modal").modal("hide");
                            ClearJobCreateFields();
                        }
                    },
                    error: function (result) {
                        console.log(result);
                    }
                });
            }
            else
            {
                alert("Choose a job type for this vacancy");
            }
        }
        else
        {
            alert("Select at least one location for this job vacancy");
        } 
    });

    $("#btn-update-vacancy").click(function () {
        var jobId = $(editObj).attr("data-vid");
        var tr = $(editObj).parent().parent();
        var table = $("#vacancy-listing").DataTable();
        var rowData = table.row(tr).data();

        var jobTitle = $("#update-vacancy-job-post-name").val();
        var jobType = $("#update-job-selector option:selected").val();
        var vacancyStatus = $("#update-vacancy-status option:selected").val();
        var _jobLocations = $("#update-locations-selector option:selected");
        var startDate = $("#update-start-date").val();
        var endDate = $("#update-end-date").val();

        var jobLocations = [];
        $(_jobLocations).each(function () {
            var location = {};
            location.locationId = $(this).val();
            location.locationName = $(this).text();
            jobLocations.push(location);
        });

        if (_jobLocations.length > 0) {
            if (jobType !== "0" && vacancyStatus !== "") {
                var json = JSON.stringify({ "employerVacancyId": jobId, "jobType": jobType, "jobTitle": jobTitle, "startDate": startDate, "endDate": endDate, "locations": jobLocations, "vacancyStatus": vacancyStatus });
                $.ajax({
                    url: "/employer/update-vacancy",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: json,
                    success: function (result) {
                        if (result.status === "job_updated") {
                            console.log(result);
                            rowData[0] = result.vacancy.jobTitle;
                            rowData[1] = result.vacancy.html_jobType;
                            rowData[2] = result.vacancy.html_locations;
                            rowData[3] = result.vacancy.html_startDate;
                            rowData[4] = result.vacancy.html_endDate;
                            rowData[5] = result.vacancy.html_vacancyStatus;
                            table.row(tr).data(rowData).invalidate();
                            $("#edit-vacancy-modal").modal("hide");
                        }
                    },
                    error: function (result) {
                        console.log(result);
                    }
                });
            }
            else {
                alert("Select a job type and the vacancyStatus");
            }
        }
        else
        {
            alert("Select at least one location for this job vacancy");
        }

        
    });

    $("#btn-close-updatevacancy").click(function () {
        $("#edit-vacancy-moda").modal("hide");
    });

  

    $("body").on("click", ".btn-edit", function () {
        editObj = $(this);
        var table = $("#vacancy-listing").DataTable();
        var rowData = table.row($(editObj).parent().parent()).data();
       
        if (rowData[5].indexOf("Closed") < 0 && rowData[5].indexOf("Filled") < 0)
        {
            RestoreVacancyOptions(editObj.attr("data-jobId"));
            $("#edit-vacancy-modal").modal("show");
        }
    });

    function RestoreVacancyOptions(jobId)
    {
        $.ajax({
            url: "/employer/restore-joblocs?jobId=" + encodeURIComponent(jobId),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            data: {},
            success: function (result) {
                console.log(result);
                var arrayValues = [];
                $.each(result.jobLocations, function (i, value) {
                    arrayValues.push(value.locationId);
                });

                $("#update-vacancy-status").val(result.vacancy.vacancyStatus);
                $("#update-locations-selector").val(arrayValues);
                $("#update-job-selector").val(result.jobType.jobId);
                $("#update-vacancy-job-post-name").val(result.vacancy.jobTitle);
                $("#update-start-date").val(result.vacancy.html_startDate);
                $("#update-end-date").val(result.vacancy.html_endDate);
            },
            error: function (result) {
                console.log(result);
            }
        });
    }

    function ClearJobCreateFields()
    {
        $("#locations-selector option:selected").removeAttr("selected");
        $("#vacancy-job-post-name").val("");
        $("#job-type-selector option:selected").removeAttr("selected");
        $("#end-date").val("");
    }
});