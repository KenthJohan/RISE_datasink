{
	const elements_input = document.querySelectorAll('input[type="datetime-local"][now]');
	//console.log(inputs);
	for (i = 0; i < elements_input.length; ++i)
	{
		const now = new Date();
		now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
		//now.setMilliseconds(null);
		elements_input[i].value = now.toISOString();
	}
}