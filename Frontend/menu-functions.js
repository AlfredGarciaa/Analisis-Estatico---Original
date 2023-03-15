window.addEventListener('DOMContentLoaded', function(event){

    function toggleFunctions(event){
        //debugger;
        const container = document.querySelector('#navigation-container .menu-functions');
        container.classList.toggle('func-on');
        /*if(toggleState = "OFF"){
            document.querySelector('#navigation-container .specific-filter-wrapper').style.display = "block";
            toggleState = "ON";
        }
        else if(toggleState === "ON" ){
            document.querySelector('#navigation-container .specific-filter-wrapper').style.display = "none";
            toggleState = "ON";
        }*/
            

    }



    let toggleState = "OFF";

    let functionBtn = document.querySelector('#navigation-container .func-toggle');
    functionBtn.addEventListener('click', toggleFunctions);

});