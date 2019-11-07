# 光环系统
视频地址：https://v.youku.com/v_show/id_XNDQyNzkzNTMzMg==.html?spm=a2h3j.8428770.3416059.1
## 实现过程
1.创建光环类，设置半径，角度，时间属性。
```
public CirclePosition(float radius, float angle, float time)
{
    this.radius = radius;   // 半径  
    this.angle = angle;     // 角度  
    this.time = time;       // 时间  
}
```
2.设定粒子数量大小和其他信息：
```
    public class ParticleHalo : MonoBehaviour {
    private ParticleSystem particleSys;  // 粒子系统  
    private ParticleSystem.Particle[] particleArr;  // 粒子数组  
    private CirclePosition[] circle; // 极坐标数组
    public int count = 10000;       // 粒子数量  
    public float size = 0.03f;      // 粒子大小  
    public float minRadius = 5.0f;  // 最小半径  
    public float maxRadius = 12.0f; // 最大半径  
    public bool clockwise = true;   // 顺时针|逆时针  
    public float speed = 2f;        // 速度  
    public float maxRadiusChange = 0.02f;  // 游离范围
    private NormalDistribution normalGenerator = new NormalDistribution(); // 高斯分布生成器
    public Color startColor = Color.blue; //初始颜色
    // Use this for initialization
    void Start()
    {   // 初始化粒子数组  
        particleArr = new ParticleSystem.Particle[count];
        circle = new CirclePosition[count];

        // 初始化粒子系统  
        particleSys = this.GetComponent<ParticleSystem>();
        var main = particleSys.main;
        main.startSpeed = 0;              
        main.startSize = size;          // 设置粒子大小  
        main.loop = false;
        main.maxParticles = count;      // 设置最大粒子量  
        particleSys.Emit(count);               // 发射粒子  
        particleSys.GetParticles(particleArr);

        RandomlySpread();   // 初始化各粒子位置  
    }
```
3.使用高斯分布使得初始化的所有粒子分布在环上：
```
using System;

public class NormalDistribution
{
    // use Marsaglia polar method to generate normal distribution
    private bool _hasDeviate;
    private double _storedDeviate;
    private readonly Random _random;

    public NormalDistribution(Random random = null)
    {
        _random = random ?? new Random();
    }

    public double NextGaussian(double mu = 0, double sigma = 1)
    {
        if (sigma <= 0)
            throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

        if (_hasDeviate)
        {
            _hasDeviate = false;
            return _storedDeviate * sigma + mu;
        }

        double v1, v2, rSquared;
        do
        {
            // two random values between -1.0 and 1.0
            v1 = 2 * _random.NextDouble() - 1;
            v2 = 2 * _random.NextDouble() - 1;
            rSquared = v1 * v1 + v2 * v2;
            // ensure within the unit circle
        } while (rSquared >= 1 || rSquared == 0);

        // calculate polar tranformation for each deviate
        var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
        // store first deviate
        _storedDeviate = v2 * polar;
        _hasDeviate = true;
        // return second deviate
        return v1 * polar * sigma + mu;
    }
}
```
4.让分布好的粒子在光环上旋转移动
```
// Update is called once per frame
    private int tier = 12;  // 速度层数  
    void Update()
    {
        for (int i = 0; i < count; i++)
        {
            if (clockwise)  // 顺时针旋转  
                circle[i].angle -= (i % tier + 1) * (speed / circle[i].radius / tier);
            else            // 逆时针旋转  
                circle[i].angle += (i % tier + 1) * (speed / circle[i].radius / tier);

            // 保证angle在0~360度  
            circle[i].angle = (360.0f + circle[i].angle) % 360.0f;
            float theta = circle[i].angle / 180 * Mathf.PI;
            //粒子在XZ平面上以半径值转圈
            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));
            particleArr[i].startColor = startColor;
            // 粒子在半径方向上游离  
            circle[i].time += Time.deltaTime;
            circle[i].radius += Mathf.PingPong(circle[i].time / minRadius / maxRadius, maxRadiusChange) - maxRadiusChange / 2.0f;
        }

        particleSys.SetParticles(particleArr, particleArr.Length);
    }

    void RandomlySpread()
    {
        for (int i = 0; i < count; ++i)
        {   
            // 使用高斯分布生成半径， 均值为midRadius，标准差为0.7
            float midRadius = (maxRadius + minRadius) / 2;
            float radius = (float)normalGenerator.NextGaussian(midRadius, 0.7);

            float angle = Random.Range(0.0f, 360.0f);
            float theta = angle / 180 * Mathf.PI;
            float time = Random.Range(0.0f, 360.0f);    // 给粒子生成一个随机的初始进度
            float radiusChange = Random.Range(0.0f, maxRadiusChange);   // 随机生成一个轨道变化大小
            circle[i] = new CirclePosition(radius, angle, time);
            particleArr[i].position = new Vector3(circle[i].radius * Mathf.Cos(theta), 0f, circle[i].radius * Mathf.Sin(theta));
        }

        particleSys.SetParticles(particleArr, particleArr.Length);
    }
}
```
