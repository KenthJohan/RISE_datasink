var element_url = document.getElementById("myurl");
element_url.value = ws_url("/wslog");
var element_status = document.getElementById("mystatus");


var myapp = {};
myapp.n = 0;
myapp.table = document.getElementById("mytable");
myapp.tbody = myapp.table.createTBody();
myapp.thead = myapp.table.createTHead();


function table_init(app)
{
	app.data = [];
	app.data[0] = [];
	app.data[1] = [];
	app.data[2] = [];
	app.data[3] = [];
	app.data[4] = [];
	app.data[5] = [];
	app.data[6] = [];
	app.data[7] = [];

	app.filter = [];
	app.input = [];

	app.colname = ["Time", "ClientIp", "RequestId", "Method", "SourceContext", "Message"];
	app.tbody.innerHTML = "";
	app.thead.innerHTML = "";
	app.thead.n = 0;
}








function table_head(app)
{
	var tr;
	var th;
	tr = app.thead.insertRow(-1);
	for (let c in app.colname)
	{
		th = table_insert_th(tr);
		th.textContent = app.colname[c];
	}
	tr = app.thead.insertRow(-1);
	for (let c in app.colname)
	{
		th = table_insert_th(tr);
		app.input[c] = document.createElement('input');
		app.input[c].style.width = "100%";
		th.appendChild(app.input[c]);
		app.input[c].oninput = function()
		{
			table_press(app, c);
		}
	}
}

function table_press(app, c)
{
	console.log(app);
	var children = app.tbody.children;
	var value = app.input[c].value;
	for (var i = 0; i < children.length; i++)
	{
		var a = app.data[c][i].includes(value);
		app.filter[i] &= ~(1 << c);
		app.filter[i] |= (a << c);
	}
	for (var i = 0; i < children.length; i++)
	{
		children[i].style.visibility = (app.filter[i] == ~0) ? "visible" : "collapse";
	}
}


function table_filter_one(app, i)
{
	var children = app.tbody.children;
	for (let c in app.colname)
	{
		var value = app.input[c].value;
		var a = app.data[c][i].includes(value);
		app.filter[i] &= ~(1 << c);
		app.filter[i] |= (a << c);
	}
	children[i].style.visibility = (app.filter[i] == ~0) ? "visible" : "collapse";
}





function getcolor(s)
{
	if (s == "Error")
	{
		return "#FAA";
	}
	else if (s == "Warning")
	{
		return "#FF7";
	}
	else if (s == "Verbose")
	{
		return "#defff8";
	}
	else if (s == "Debug")
	{
		return "#f4edff";
	}
	return "#EEE";
}


function app_make_onclick(app, c, n)
{
	return function()
	{
		if (app.input[c].value)
		{
			app.input[c].value = "";
		}
		else
		{
			app.input[c].value = app.data[c][n];
		}
		app.input[c].oninput();
	}
}



function app_append(app, row)
{
	//console.log(app.data);
	//["Time", "ClientIp", "RequestId", "Method", "SourceContext", "Message"];
	app.data[0][app.n] = formatdate(new Date(row["@t"]));
	app.data[1][app.n] = row["ClientIp"] ? row["ClientIp"] : "";
	app.data[2][app.n] = row["RequestId"] ? row["RequestId"] : "";
	app.data[3][app.n] = row["Method"] ? row["Method"] : "";
	app.data[4][app.n] = row["SourceContext"] ? row["SourceContext"] : "";
	app.data[5][app.n] = row["@m"] ? row["@m"] : "";

	app.data[6][app.n] = new Date(row["@t"]);
	app.data[7][app.n] = row["@l"];

	app.filter[app.n] = ~0;

	let tr;//Table row
	let td;//Table cell
	tr = app.tbody.insertRow(-1);
	//td = tr.insertCell(-1);
	//td.textContent = data["@l"] ? data["@l"] : "Information";


	td = tr.insertCell(-1);
	td.textContent = app.data[0][app.n];
	td.style.backgroundColor = random_hsla(td.textContent);
	td.onclick = app_make_onclick(app, 0, app.n);

	td = tr.insertCell(-1);
	td.textContent = row["ClientIp"];
	td.style.backgroundColor = random_hsla(row["ClientIp"]);
	td.onclick = app_make_onclick(app, 1, app.n);

	td = tr.insertCell(-1);
	td.textContent = row["RequestId"];
	td.style.backgroundColor = random_hsla(row["RequestId"]);
	td.onclick = app_make_onclick(app, 2, app.n);

	td = tr.insertCell(-1);
	td.style.width = "50px"
	if(row["Method"])
	{
		td.textContent = row["Method"];
		td.style.backgroundColor = random_hsla(row["Method"]);
	}
	td.onclick = app_make_onclick(app, 3, app.n);

	td = tr.insertCell(-1);
	td.textContent = row["SourceContext"];
	td.style.backgroundColor = random_hsla(row["SourceContext"]);
	td.onclick = app_make_onclick(app, 4, app.n);

	td = tr.insertCell(-1);
	td.textContent = row["@m"];
	td.style.backgroundColor = getcolor(row["@l"]);
	td.onclick = app_make_onclick(app, 5, app.n);

	//if (data["@mt"])
	{
		//td.textContent = template(data["@mt"], data);
	}
	
	table_filter_one(app, app.n);
	app.n++;
}


function ws_connect()
{
	console.log(element_url.value);
	socket = new WebSocket(element_url.value);
	socket.onopen = function (event)
	{
		table_init(myapp);
		table_head(myapp);
	};
	socket.onclose = function (event)
	{

	};
	socket.onerror = function (event)
	{
		console.log(event);
		element_status.innerText = "Unknown Error. You need to be /Siteadmin to connect to websocket.";
	};
	socket.onmessage = function (event)
	{
		var row = JSON.parse(event.data);
		console.log(row);
		app_append(myapp, row);
	};
};


ws_connect();