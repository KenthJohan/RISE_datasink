var element_main = document.getElementById("main");


function load_devices()
{

}



function hashchange()
{
	var h = location.hash.substring(1).split('/');
	console.log(h);
	switch(h[0])
	{
	case "serie":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{series{id name device{name} location{name} project{name} quantity{name}}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_series(x));
			});

		}
		break;
	case "device":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{devices{id name}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_devices(x));
			});

		}
		break;
	case "project":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{projects{id name}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_projects(x));
			});

		}
		break;

	case "location":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{locations{id name}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_locations(x));
			});

		}
		break;

	case "quantity":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{quantities{id name}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_quantities(x));
			});

		}
		break;

	case "seriefloats":
		if (!/\D/.test(h[1]))
		{
			//load_user(target, h[1]);
		}
		else
		{
			make_request("query{seriefloats{time value}}", (x)=>{
				//let data = [{column1:"Bananas"},{column1:"Kiwi"}];
				element_main.innerHTML = "";
				element_main.appendChild(html_seriefloats(x));
			});

		}
		break;
	}
}


window.addEventListener('DOMContentLoaded', hashchange);
window.addEventListener('hashchange', hashchange, false);

