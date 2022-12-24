# Top Ten Hashtags

An exercise in .NET API access

## Rationale

I did this as an off-season coding exercise, and to show off my skils to a certain organization that shall remain nameless.

To the representatives of that organization, I do apologize for the delay in making this pubic, as the holiday season has taken its toll on my free time.

## Setup

At the risk of falling back on the old saw, "It Works On My Box", this application requires a Twitter API key, or more precisely, the Bearer token you receive with your key when you register with Twitter at <https://developer.twitter.com>.

The application should launch in the Development environment, which will allow you to place the Bearer token in your local User Secrets setup (see <https://dotnetcoretutorials.com/2022/04/28/using-user-secrets-configuration-in-net/> for more information). You can add your Bearer token from the command line with

```bash
dotnet user-secrets set "TwitterApi:BearerToken" "<insert bearer token here>"
```

Do NOT include the word "Bearer" in the value. The application will do this for you.

## Operation

All running parts of the application are included in a single project (`TopTenHashTags`), with a small test suite in the other project (named, perhaps unimaginatively, `TopTenHashTags.Tests`). Simply start the `TopTenHashTags` project, and open a browser to <http://localhost:5000/swagger/index.html>. As you can see, there's one GET endpoint, `/TopTenHashTags`, which you can try out. There are no parameters, and it simply returns a JSON string of the top ten hashtags as of the time of the call. That's about it!

To stop the application, simply type Ctrl-C in the application's console window.

Thanks for looking in!

_Eric Lloyd_
_December 23, 2022_
