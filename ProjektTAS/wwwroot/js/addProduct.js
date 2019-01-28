function addProduct() {

    var category_id = document.getElementById("dropdown");
    var catid = category_id.options[category_id.selectedIndex].getAttribute('data-value');
    var product_name = document.getElementById("nazwa").value;
    var price = document.getElementById("cena").value;

    $('.ui.form')
        .form({
            on: 'blur',
            fields: {
                category_id: {
                    identifier: 'dropdown',
                    rules: [{
                        type: 'empty',
                        prompt: 'Nie wybrano kategorii.'
                    }]
                },
                product_name: {
                    identifier: 'nazwa',
                    rules: [{
                        type: 'empty',
                        prompt: 'Nie podano nazwy.'
                    },
                    {
                        type: 'minLength[6]',
                        prompt: 'Nazwa musi zawieraæ co najmniej 6 znaków'
                    }]
                },
                price: {
                    identifier: 'cena',
                    rules: [{
                        type: 'decimal',
                        prompt: 'Nie podano ceny produktu lub jest nieprawid³owa.'
                    }]
                }
            }
        });

    var token = getCookie('SessionToken');
    var xhr = new XMLHttpRequest();
    var url = "/rest/v1/product/create";

    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.setRequestHeader("Authorization", "Bearer " + token.replace(/['"]+/g, ''));
 
    xhr.onreadystatechange = function () {
        if (xhr.status === 200) {
            window.location = "http://localhost:48013/Index";
        }
    };
    
    


    var data = JSON.stringify({ "CategoryId": catid, "Name": product_name, "Price": price });
    
    xhr.send(data);

}