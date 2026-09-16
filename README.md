# Guts-DotNetAdvanced

In this repository you can find (visual studio) start solutions for the lab exercises of the **.NET Advanced** course of **PXL-Digital**.

An exercise solution usually contains multiple projects:

- One or more project for the exercise. E.g. _CardGames.Desktop_ and _CardGames.Domain_
- One or more test projects for the exercise projects. E.g. _CardGames.Domain.Tests_

![alt text][img_projects]

The exercise projects are (mostly) empty and waiting for you to complete them.
The matching test projects contain **automated tests** that can be run to check if your solution is correct.

## Getting Started

### Clone the repository

After following the **PXL classroom instructions on BlackBoard** you have an online fork of the repository containing the startcode.

Next you need to clone the files in your online repository to your local machine.
You will use the Visual Studio **git** capabilities to accomplish this.
Start Visual Studio and select "Clone or check out code".

- Click on the _Clone or Download_ button in the upper right corner of this webpage
- Copy the url of this repository

![alt text][img_clone_url]

- Paste the url you copied earlier into the field "Repository Location"
- Choose a local path, this is a local folder where the files will be copied to
- Click on the _Clone_ button

![alt text][img_clone_vs]

Now you have a local copy of the online repository in which you can complete your exercises.

Double click the solution that contains the exercises you want to work on. Alternatively, you can just double click a solution file (.sln) from an explorer window to start up Visual Studio with the solution opened up.

### New exercises and bugfixes

Exercises will be added and changes will be made during the course. These changes will happen in the original repository made by the lectors (your online repository is a fork of this repository). We will call the original repository of the lectors the **upstream** repository.
Your personal online repository will be called the **origin** from now on.

When the lector pushes changes in the **upstream** to the student repo's (your origin) one of two things can happen:

- There are **no conflicts** and the commit(s) of the lector are added to your repo. You only need to **pull** the changes in the clone of the repo on your machine.
- When there are **conflicts** with existing commits a Pull Request (PR) is created. This gives you the chance to pull the changes into your local clone, resolve the conflicts and merge the changes into the main branch of your **origin**. Ask for the lector's assistance if you don't know how to work with PR's.

![alt text][img_pr]

### Register on [guts-web.pxl.be](https://guts-web.pxl.be)

To be able to send your tests results to the Guts servers you need to register via [guts-web.pxl.be](https://guts-web.pxl.be/register).
After registration you will have the credentials you need to succesfully run automated tests for an exercise.

#### Start working on an exercise

1. Open the solution of the exercise. You can do this by doubleclicking on the **.sln** file from an explorer window or by opening visual studio, clicking on _File &rightarrow; Open a project or solution_ and selecting the **.sln** file.

2. **Build the solution** (Menu: Build &rightarrow; Build Solution or Ctrl+Shift+B)

3. Write the code you need to write

#### Run the automated tests

1. Open the _Test Explorer_ window (Menu: Test &rightarrow; Test Explorer)
2. In the top right corner, click on the _group by_ button and make sure the automated tests are grouped by project (see the picture below). If you don't see any tests appearing, you probably should (re)build your solution.

![alt text][img_group_tests]

3. Right click on the project that matches your exercise and click on _Run_ to execute the tests.
4. The first time you run a test a browser window will appear asking you to log in. You should fill in your credentials from [guts-web.pxl.be](https://guts-web.pxl.be).

![alt text][img_login_vs]

##### FAQ

**Why won't my tests run?**

The first time it can happen that you see the tests in the _Test Explorer_ but if you run the tests, nothing happens.
Try to clean your solution (**Build &rightarrow; Clean Solution**) and then to rebuild your solution (**Build &rightarrow; Rebuild solution**).

**Why can't I see my test results on the Guts website? Locally all my tests are green.**

After the tests are run, the testrunner will try to send your results to the server. In the _Output Window_ you can see a log of the steps that are taken.
If anything goes wrong, you should be able to find more info in the _Output Window_.

The test results will only be sent to the server when you run all te tests of an exercise at once. If you run the tests one by one the results will not be sent to the servers.

#### Inspect the test results

Tests that pass will be green. Tests that don't pass will be red.

The _name of the test_ gives an indication of what is tested in the automated test.
If you click on a test you can also read more detailed messages that may help you to find out what is going wrong.

![alt text][img_test_detail]

Although it is not a guarantee, having all tests green is a good indication that you completed the exercise correctly.

#### Check your results online

Test results of all students are sent to the Guts servers.
You can check your progress and compare with the averages of other students via [guts-web.pxl.be](https://guts-web.pxl.be).
Login, go to ".NET Advanced" in the navigation bar and select the chapter (module) you want to view.

![alt text][img_chapter_contents]

#### Save (commit) your work

It could happen that the code in the online repository changes and that you need to pull (download) a new version of the start code in your local repository.
The online repository does not contain your solutions. Pulling a new version of the code could result in you losing your work.

To avoid this you should regularly commit (save) your work in your local git database. If you have commited your work an you pull a new version, git will be able to automatically merge your work with the online changes.
It is recommended to **do a git commit every time you complete an exercise**.

- Go to _Git Changes_

![alt text][img_git_changes]

- In the _Git Changes_ screen you get an overview of the changes you made locally. Fill in a commit message (describing what you did) and click on the _Commit All_ button. Your changes are now saved in your local git database.

![alt text][img_commit_your_work]

- By clicking on the _Solution Explorer_ tab you go back to the main view for this local repository

[img_projects]: Images/projects.png "Solution for chapter five with its projects"
[img_clone_vs]: Images/clone_vs.png "Clone a project in Visual Studio"
[img_clone_url]: Images/clone_url.png "Copy repository url"
[img_group_tests]: Images/group_tests.png "Group tests by project"
[img_test_detail]: Images/test_detail.png "Details of a test result"
[img_login_vs]: Images/login_vs.png "Visual studio login"
[img_chapter_contents]: Images/chaptercontents.png "Chapter contents"
[img_commit_your_work]: Images/commit_your_work.png "Commit your work"
[img_git_changes]: Images/git_changes.png "Git Changes"
[img_pr]: Images/pr.png "Pull Request"
