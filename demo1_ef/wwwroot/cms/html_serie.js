function html_series(data)
{
	var element = document.createElement('div');
	var element_title = document.createElement('h2');
	element_title.textContent = "Series";
	element.appendChild(element_title);
	element.appendChild(html_series_table(data.series));
	return element;
}



function html_series_table(rows)
{
	assert(Array.isArray(rows));

	var element = document.createElement('table');
	var thead = element.createTHead();
	var tbody = element.createTBody();
	var td;
	var tr;
	var th;

	tr = thead.insertRow(-1);
	
	th = document.createElement('th'); 
	th.textContent = "Id";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Name";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Device";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Project";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Quantity";
	tr.appendChild(th);

	th = document.createElement('th'); 
	th.textContent = "Location";
	tr.appendChild(th);

	for (let i = 0; i < rows.length; ++i)
	{
		tr = thead.insertRow(-1);

		td = tr.insertCell(-1);
		td.textContent = rows[i].id;
		td = tr.insertCell(-1);
		td.textContent = rows[i].name;
		td = tr.insertCell(-1);
		td.textContent = rows[i].device.name;
		td = tr.insertCell(-1);
		td.textContent = rows[i].project.name;
		td = tr.insertCell(-1);
		td.textContent = rows[i].quantity.name;
		td = tr.insertCell(-1);
		td.textContent = rows[i].location.name;
	}

	return element;
}