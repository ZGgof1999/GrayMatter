# GrayMatter
A Tiny Neural Interface for Building AI Agents in Unity

Virtual Environment -> Receptor -> Brain -> Muscle -> Virtual Environment

The currently intended method of training is to simulate multiple organisms with a fitness function, while feeding the top scoring Brains into a new child Brain as its Parent. There is support for custom mutation rates, and activation functions of the neural network through lambdas. The preprocessing / data gathering function in the Receptor, as well as the Action function of the Muscle, are also set to custom functions that interact with the virtual environment through a lambda.
