function SalesOrderDetailsViewModel(app, dataModel) {
    var self = this;
    self.RecommendationsList = ko.observable("");
    self.recommendationUrl = 'Recommendations';
    $(function () {
        $("#btnAdd").click(function (e) {
            var container = document.getElementById('ProductsSection');
            var clone = document.getElementById('ProductIdDiv').cloneNode(true);
            clone.setAttribute('id', 'div_' + document.getElementById('ProductIdDiv').getElementsByTagName('div').length);
            container.appendChild(clone);
        });
    });
    Sammy(function () {
        $("#btnGetReco").click(function (e) {
            var csvList = '';
            $("#ProductsSection").children().each(function(){
                var DOMelement = this;
                var jQueryElement = $(this);
                if (jQueryElement.children()[1] && jQueryElement.children()[1].children[0].value) {
                    csvList = csvList + jQueryElement.children()[1].children[0].value + '-';
                }
            });

            $.ajax({
                method: 'get',
                url: self.recommendationUrl,
                data: { commaSeparatedItemIds: csvList },
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    $('#RecommendationsTable').replaceWith('<div id="RecommendationsTable">' + data + '</div>');
                }
            });
        });
    });
    btnGetReco
    return self;
}

app.addViewModel({
    name: "SalesOrderDetails",
    bindingMemberName: "salesorderdetails",
    factory: SalesOrderDetailsViewModel
});