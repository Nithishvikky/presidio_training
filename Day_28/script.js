const users = [
      { id: 1, name: "nithish", age: 20 },
      { id: 2, name: "vignesh", age: 20 },
      { id: 3, name: "virat", age: 36 }
    ];
    
function getUsersCallback(callback) {
    setTimeout(() => {
        callback(users);
    }, 1000);
}

document.getElementById('Btn1').addEventListener('click', () => {
    document.getElementById('Container').innerHTML = 'Loading...';
    getUsersCallback((data) => {
        displayUsers(data);
    });
});

    
function getUsersPromise() {
    return new Promise((resolve) => {
        setTimeout(() => {
          resolve(users);
        }, 1000);
    });
}

document.getElementById('Btn2').addEventListener('click', () => {
    document.getElementById('Container').innerHTML = 'Loading...';
    getUsersPromise().then(data => displayUsers(data));
});

    
async function getUsersAsync() {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve(users);
        },  1000);
    });
}

document.getElementById('Btn3').addEventListener('click', async () => {
    document.getElementById('Container').innerHTML = 'Loading...';
    const data = await getUsersAsync();
    displayUsers(data);
});


function displayUsers(data) {
    const DisplayContainer = document.getElementById('Container');
    document.getElementById('Container').innerHTML = '';
    data.forEach(user => {
        const userDiv = document.createElement('div');
        userDiv.innerText = `Username: ${user.name}, Age: ${user.age}`;
        DisplayContainer.appendChild(userDiv);
    });
}