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
















function gql_checkbox_update(table, id, column, value)
{
	let query = `mutation{${table}_update(id:${id},${column}:${value})}`;
	var xhr = new XMLHttpRequest();
	xhr.open("POST", window.location.origin + "/graphql", true);
	xhr.setRequestHeader("Content-Type", "application/json");
	xhr.onload = function ()
	{
		var response = JSON.parse(xhr.response);
		if (response.data === null) { return; }
	};
	xhr.onerror = function ()
	{
		console.log("XHR unknown error. Probobly Cross-Origin Request Blocked");
	}
	var data = JSON.stringify({ "query": query });
	xhr.send(data);
}












function build_column_th(tr, column)
{
	var toname = {};
	toname['enable_mqtt'] = 'MQTT';
	toname['enable_reqget'] = 'GET';
	var th = document.createElement("th");
	th.textContent = toname[column] ? toname[column] : column;
	switch(column)
	{
	case 'id':
	case 'name':
	case 'email':
		break;
	case 'enable_mqtt':
	case 'enable_reqget':
		th.style.writingMode = "vertical-lr";
		break;
	}
	tr.appendChild(th);
}

function build_column(table, tr, column, rows, r)
{
	var td = tr.insertCell(-1);
	switch(column)
	{
	case 'id':
	case 'name':
	case 'email':
		//td.style.width = "30px";
		td.textContent = rows[r][column];
		break;
	case 'enable_mqtt':
	case 'enable_reqget':
		var checkbox = document.createElement("input");
		checkbox.setAttribute("type", "checkbox");
		checkbox.checked = rows[r][column];
		checkbox.onchange = (x) => 
		{
			gql_checkbox_update(table, rows[r].id, column, x.target.checked);
		}
		td.appendChild(checkbox);
		break;
	}
}

function build_tables(configs, data)
{
	for (const k in configs)
	{
		configs[k].dst = document.getElementById(k);
		build_table(configs[k], data[k]);
	}
}

function build_table(config, rows)
{
	
	{
		var thead = document.createElement('thead');
		var tr = thead.insertRow(-1);
		var th = document.createElement("th");
		th.colSpan = 100;
		th.textContent = config.title;
		tr.appendChild(th);
		config.dst.appendChild(thead);
	}

	{
		var thead = document.createElement('thead');
		var tr = thead.insertRow(-1);
		for(var i = 0; i < config.columns.length; ++i)
		{
			build_column_th(tr, config.columns[i]);
		}
		config.dst.appendChild(thead);
	}

	{
		var tbody = document.createElement('tbody');
		for(var r = 0; r < rows.length; ++r)
		{
			var tr = tbody.insertRow(-1);
			for(var c = 0; c < config.columns.length; ++c)
			{
				build_column(config.table, tr, config.columns[c], rows, r);
			}
		}
		config.dst.appendChild(tbody);
	}
}


/*
var query = `query{
t1:projects{id name}
t2:quantities{id name}
t3:producers{id name enable_mqtt enable_reqget}
t4:locations{id name}
t5:users{id email}
t6:memlocs{id}
t7:layouts{id name}
t8:devices{id name}
}`;
*/

function build_query(configs)
{
	let query = 'query{\n';
	for (const k in configs)
	{
		query += `${k}:${configs[k].table}{`;
		for (const r in configs[k].columns)
		{
			query += configs[k].columns[r] + ' ';
		}
		query += '}\n';
	}
	query += '}';
	//console.log(query);
	return query;
}