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
	toname['enable_websock'] = 'WS';
	toname['enable_reqjson'] = 'JSON';
	toname['enable_tcp'] = 'TCP';
	toname['enable_udp'] = 'UDP';
	toname['enable_storage'] = 'DB';
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
	case 'enable_websock':
	case 'enable_reqjson':
	case 'enable_tcp':
	case 'enable_udp':
		th.style.backgroundColor = "#FAA";
	case 'enable_storage':
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
	case 'layout_id':
	case 'producer_id':
		//td.style.width = "30px";
		td.textContent = rows[r][column];
		break;
	case 'enable_mqtt':
	case 'enable_reqget':
	case 'enable_websock':
	case 'enable_reqjson':
	case 'enable_tcp':
	case 'enable_udp':
	case 'enable_storage':
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