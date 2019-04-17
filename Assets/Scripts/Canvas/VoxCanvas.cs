using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxCanvas : MonoBehaviour {

    private abstract class VoxelDelta {
        public enum Type {Set, Add, Remove }
        public Type type { get; protected set; }
        public Vector3Int pos { get; protected set; }
    }

    private class AddDelta : VoxelDelta {
        public Color color { get; private set; }
        public uint id { get; private set; }
        public AddDelta (Vector3Int pos, Color color, uint id) {
            this.pos = pos;
            this.color = color;
            this.id = id;
            type = Type.Add;
        }
    }

    private class SetDelta : VoxelDelta {
        public Color color { get; private set; }

        public SetDelta(Vector3Int pos, Color color) {
            this.pos = pos;
            this.color = color;
            type = Type.Set;
        }
    }

    private class RemoveDelta : VoxelDelta {

        public RemoveDelta (Vector3Int pos) {
            this.pos = pos;
            type = Type.Remove;
        }
    }

    public Transform Root;
    public ParticleSystem system;
    public int Scale;

    private ParticleSystem.Particle defaultParticle;
    private ParticleSystem.Particle deadParticle;

    private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[65535 / 12];

    private Dictionary<Vector3Int, uint> idMap;
    private Dictionary<uint, VoxelDelta> pendingDeltas;
    private List<AddDelta> pendingAdds;

    uint nextId;

    void Awake () {
        defaultParticle = new ParticleSystem.Particle {
            remainingLifetime = float.MaxValue,
            startSize = 1
        };

        deadParticle = new ParticleSystem.Particle {
            remainingLifetime = -1,
            startSize = 0
        };

        idMap = new Dictionary<Vector3Int, uint> ();
        pendingDeltas = new Dictionary<uint, VoxelDelta> ();
        pendingAdds = new List<AddDelta> ();
        nextId = 0;
    }

    void Start () {
        for (int i = 0; i < 5; i++) {
            Place (new Vector3Int (i, 10, 0), Color.red);
        }
    }

    void LateUpdate () {

        int count = system.GetParticles (particleBuffer);
        
        for (int i = 0; i < pendingAdds.Count; i++) {

            AddDelta a = pendingAdds[i];
            defaultParticle.startColor = a.color;
            defaultParticle.position = a.pos;
            defaultParticle.randomSeed = pendingAdds[i].id;
            particleBuffer[i + count] = defaultParticle;

        }

        if (pendingDeltas.Count > 0) {
            for (int i = 0; i < count; i++) {

                uint id = particleBuffer[i].randomSeed;
                if (pendingDeltas.ContainsKey (id)) {
                    VoxelDelta d = pendingDeltas[id];

                    switch (d.type) {
                    case VoxelDelta.Type.Remove:

                        particleBuffer[i] = deadParticle;

                        break;
                    case VoxelDelta.Type.Set:

                        SetDelta s = d as SetDelta;
                        defaultParticle.startColor = s.color;
                        defaultParticle.position = s.pos;
                        particleBuffer[i] = defaultParticle;

                        break;
                    }
                    
                }
            }
        }
        count += pendingAdds.Count;

        pendingAdds.Clear ();
        pendingDeltas.Clear ();

        system.SetParticles (particleBuffer, count);
    }

    public bool Place(Vector3Int pos, Color color) {
        uint id;
        if (idMap.ContainsKey (pos)) {
            id = idMap[pos];
            pendingDeltas[id] = new SetDelta (pos, color);
            return false;
        } else {
            id = nextId;
            nextId++;
            idMap[pos] = id;
            pendingAdds.Add(new AddDelta (pos, color, id));
            return true;
        }
    }

    public bool Remove(Vector3Int pos) {
        if (!idMap.ContainsKey (pos))
            return false;

        uint id = idMap[pos];

        pendingDeltas[id] = new RemoveDelta (pos);
        idMap.Remove (pos);
        return true;
    }

}