function submit_callback(event)
{
	event.preventDefault();
	var query = "mutation{" + event.srcElement.getAttribute("graphql") + "(";

	{
		const data = new FormData(event.srcElement)
		console.log(data);
		for (var pair of data.entries()) {
			query += pair[0] + ":" + pair[1] + ",";
		}
		query = query.slice(0, -1);
		query += ")}";
		console.log(query);
	}

	{
		var f = Math.random();
		var xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = function () {
			var r = JSON.parse(xhr.response);
			if (r.data === null) { return; }
			//TODO: Generate better log
			if (event.srcElement.elements.log) {
				event.srcElement.elements.log.value = xhr.response;
			}
		};
		xhr.onerror = function () {
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		}
		var data = JSON.stringify({ "query": query });
		xhr.send(data);
	}
}


{
	const elements_form = document.querySelectorAll('form[graphql]');
	//console.log(forms);
	for (i = 0; i < elements_form.length; ++i) {
		elements_form[i].addEventListener('submit', submit_callback);
	}
}




{
	const elements_input = document.querySelectorAll('input[type="datetime-local"][now]');
	//console.log(inputs);
	for (i = 0; i < elements_input.length; ++i) {
		const now = new Date();
		now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
		//now.setMilliseconds(null);
		elements_input[i].value = now.toISOString();
	}
}



















{
	let elements_options = document.querySelectorAll('select[graphql_option]');
	if (elements_options.length > 0)
	{

		let elements_options1 = {};
		let query = "query{\n";
		for (let i = 0; i < elements_options.length; ++i)
		{
			const name = elements_options[i].getAttribute("name");
			const q = elements_options[i].getAttribute("graphql_option");
			elements_options1[name] = elements_options[i];
			query += name + ":" + q + "\n";
		}
		query += "}";
		//console.log(elements_options1, query);
		{
			var xhr = new XMLHttpRequest();
			xhr.open("POST", window.location.origin + "/graphql", true);
			xhr.setRequestHeader("Content-Type", "application/json");
			xhr.onload = function ()
			{
				var r = JSON.parse(xhr.response);
				if (r.data === null) { return; }
				for (const property in r.data)
				{
					//console.log(property, r.data[property]);
					for (i = 0; i < r.data[property].length; ++i)
					{
						var element_option = document.createElement('option');
						element_option.value = r.data[property][i].value;
						element_option.textContent = r.data[property][i].name + (r.data[property][i].name1 ? " (" + r.data[property][i].name1 + ")" : "");
						elements_options1[property].appendChild(element_option);
					}
				}
			};
			xhr.onerror = function ()
			{
				console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
			}
			var data = JSON.stringify({ "query": query });
			xhr.send(data);
		}
	}
}











{



	function submit(event)
	{
		event.preventDefault();
		//console.log(event.target.elements);
		log.textContent = `Form Submitted! Time stamp: ${event.timeStamp}`;
		var formdata = new FormData(event.target);
		var qmutation = event.target.getAttribute("graphql_mutation");
		var query = 'mutation{' + qmutation + '(' + gql_formdata_string(formdata) + '){id}}';
		var cbname = event.target.getAttribute("callback");
		//console.log(cbname, q);


		var xhr = new XMLHttpRequest();
		xhr.open("POST", window.location.origin + "/graphql", true);
		xhr.setRequestHeader("Content-Type", "application/json");
		xhr.onload = function ()
		{
			var r = JSON.parse(xhr.response);
			if (r.data === null) { return; }
			window[cbname](r.data);
		};
		xhr.onerror = function ()
		{
			console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
		}
		var data = JSON.stringify({ "query": query });
		xhr.send(data);

	}

	let elements_form = document.querySelectorAll('form[graphql_mutation]');
	for (let i = 0; i < elements_form.length; ++i)
	{
		elements_form[i].addEventListener('submit', submit);
	}
}





























function build_column_th(tr, column)
{
	var th;
	switch(column)
	{
	case 'id':
		th = document.createElement("th");
		th.textContent = "id";
		tr.appendChild(th);
		break;
	case 'name':
		th = document.createElement("th");
		th.textContent = "name";
		tr.appendChild(th);
		break;
	}
}

function build_column(tr, column, a)
{
	var td;
	switch(column)
	{
	case 'id':
		td = tr.insertCell(-1);
		td.textContent = a;
		td.style.width = "30px";
		break;
	case 'name':
		td = tr.insertCell(-1);
		td.textContent = a;
		td.style.width = "30px";
		break;
	}
}



function build_table(destination, q, config)
{
	
	{
		var thead = document.createElement('thead');
		var tr = thead.insertRow(-1);
		var th = document.createElement("th");
		th.colSpan = 100;
		th.textContent = config.title;
		tr.appendChild(th);
		destination.appendChild(thead);
	}

	{
		var thead = document.createElement('thead');
		var tr = thead.insertRow(-1);
		for(var i = 0; i < config.columns.length; ++i)
		{
			build_column_th(tr, config.columns[i]);
		}
		destination.appendChild(thead);
	}

	{
		var tbody = document.createElement('tbody');
		for(var r = 0; r < 4; ++r)
		{
			var tr = tbody.insertRow(-1);
			for(var c = 0; c < config.columns.length; ++c)
			{
				build_column(tr, config.columns[c], r);
			}
		}
		destination.appendChild(tbody);
	}
}