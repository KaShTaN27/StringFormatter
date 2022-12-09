namespace Tests.Model; 

public class Motorbike {

    public string Model { get; }
    private int engineVolume;

    public Motorbike(string model, int engineVolume) {
        this.engineVolume = engineVolume;
        Model = model;
    }
}