## Behaviour Tree

### To Use

1. Create new class deriving from BehaviourTree
2. Override `Start` method (optional) and initiate blackboard values, make sure to call `base.Start();` at the start of the method
3. Override `SetRoot` method, here you can build up your behaviour tree using the `BehaviourTreeBuilder` class as shown below
4. Attach the script to the GameObject
5. The nodes `Execute` method will be called every frame

### Example

```c#
using Splatter.AI.BehaviourTree;

public class ZombieBehaviourTree : BehaviourTree {
    public override void Start() {
        base.Start();
        
        Blackboard[ZombieKey] = GetComponent<Zombie>();
    }
    
    public override Node SetRoot() {
        return new BehaviourTreeBuilder(this)
            .Sequence(resetIfInterrupted: false)
            	.Do(() => {
                    // Custom action
                    return NodeResult.Success;
                })
            .End()
            .Build();
    }
}
```

