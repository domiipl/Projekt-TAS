function Validator() {
    var login = document.getElementById("login").value;
    var password = document.getElementById("password").value;
    var email = document.getElementById("email").value;
    $('.ui.form')
        .form({
            fields: {
                login: {
                    identifier: 'login',
                    rules: [{
                        type: 'regExp',
                        value: /^[A-Za-z0-9]+/i,
                        prompt: 'Login musi się składać z co najmniej jednej litery lub cyfry'
                    },
                    {
                        type: 'minLength[6]',
                        prompt: 'Login musi zawierać co najmniej 6 znaków'
                    }]
                },
                email: {
                    identifier: 'email',
                    rules: [{
                        type: 'regExp',
                        value: /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
                        prompt: 'Niepoprawny adres e-mail'
                    }]
                },
                password: {
                    identifier: 'password',
                    rules: [
                        {
                            type: 'minLength[6]',
                            prompt: 'Hasło musi posiadać co najmniej 6 znaków'
                        },
                        {
                            type: 'regExp',
                            value: /^[A-Za-z0-9]+/i,
                            prompt: 'Hasło akceptuje wyłącznie cyfry oraz małe i duże litery'
                        }
                    ]
                }
            }
        }).api({
            url: '/rest/v1/user/create',
            method: 'POST',
            beforeXHR: (xhr) => {
                xhr.setRequestHeader('Content-Type', 'application/json');
            },
            beforeSend: function (settings) {
                settings.data = JSON.stringify({ Login: login, Password: password, Email: email });
                return settings;
            },
            onSuccess: function (result) {
                window.location = "/Index?registered=true";
            },
            onFailure: function (result) {
                window.location = "/Rejestracja?registered=false";
            }
        });
}
