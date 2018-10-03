
public class HowToSetUpAWeapon {
    /*
     * This script should be attached to the DefaultWeapon Prefab under the Default folder in the Weapons folder under Assets (Assets>Weapons>Default>DefaultWeapon)
     * These are the basic instructions on how to use the systems that are currently in place. You might also seee a default weapon pickup, ignore that for now.
     * 
     * If you look at the components attached to the default weapon, you'll see a few things: PickupProperties, AudioSource, Animator, GunShoot, ProjectileShoot, ShotgunShoot,
     * WeaponProperties, and lastly, this script. 
     * 
     * Pickup Properties should already be set up, but in case its not, there are two updatable fields on it: Actual Gun Prefab and Ammo. There should be an Ammo gameObject 
     * in the heirarchy under Default weapon. If the Ammo variable is blank, click and drag that to add it to the Ammo variable. The ActualGunPrefab should be set to 
     * whatever you Change DefaultWeapon Pickup to
     * 
     * AudioSource is the central controller for any noise the weapon makes, which is usually one, the shoot/attack sound. Leave audioclip, ouput, mute, bypass effects, 
     * bypass listener effectse, bypass reverb zone, play on awake, loop, priority, stereo pan, and reverb zone mix blank. Meaning, only mess with the Volume and the Pitch.
     * Volume is how loud the shoot clip will sound when played. Pitch is the pitch of the clip
     * 
     * If you are not animating for this project, then leave Animator alone. If you are animating, you should already know what it does and how to use it. 
     * 
     * GunShoot is one of three shoot scripts. Gunshoot fires a single bullet every x seconds, with x being the fireRate. It fires from the center of the player's camera,
     * out in a random direction in a cone. It is hitscan, so the bullets don't have travel time. The only things to mess with in this is the Anim, Shot, and Particle. If they
     * are blank, then click and drag the DefaultWeapon under the Anim Slot, whatever sound you want for the shot sound in the sound slot, and either the default HitParticle or
     * a custom made particle in the Particle slot. 
     * 
     * Projectile is the second of the shoot scripts. It fires a single projectile every x seconds, with x being the fireRate. The projectile is spawned an the ProjectileSpawn,
     * and adds force in the direction the player is pointing. Note that the projectile will not always go directly to where the player's center of the screen is pointing, but instead
     * in that direction. Once the projectile is fired, ProjectileShoot has nothing to do with the projectile's behaviour. So if you want your projectile to explode, or make smoke, 
     * then add scripts to the projectile. If ProjectileSpawn is blank, then it should be set to a gameObject called projectile spawn that is a child of the weapon. Gun anim is the
     * same as in GunShoot, as is GunShot. You don't need to mess with anythign else
     * 
     * ShotgunShoot, much like the name suggests, shoots a shotgun spread ever x seconds, with x being fireRate. The amount of pellets in the spread is defined by numOfPellets, 
     * and the damage each pellet does is defined by damage. ShotgunShoot is also a hitscan, so each pellet doesn't have a travel time. However, projectile funcitonality will be added
     * later in development. Same as GunShoot, only change Gun anim, gunShot, and HitParticle if any of them are blank. 
     * 
     * WeaponProperties holds all of the changable variables required by all of the Shoot Scripts. However, some variables are only neccessary with some scripts. 
     * GunShoot requries:
     *      Fire Rate
     *      Max Magazine Size
     *      Damage
     *      Zoom Amount
     *      Ammo Limit
     *      Crit Hit Multiplier
     * Projectile Shoot requires:
     *      Fire Rate
     *      Max Magazine Size
     *      Damage
     *      Zoom Amount
     *      Ammo Limit
     *      Projectile Speed
     *      Projectile Weight
     *      Projectile
     *      Blast Radius
     * Shotgun Shoot requires:
     *      Fire Rate
     *      Max Magazine Size
     *      Damage
     *      Zoom Amount
     *      Crit Hit Multiplier
     *      Num of Pellets
     *      
     * Fire Rate- The time, in seconds, between each shot. So a fireRate of .5 would be two shots a second. 
     * Max Magazine Size- The maximum a magazine can be. When reloading, if theres enough ammo, this is what the current magazine is set to This can be increased through code. 
     * Damage- the damage either the bullet does, each pellet does, or the projectile does. Keep in mind that each enemy will have around 100-300 health. 
     * Zoom Amount- The amount that the camera will zoom in when aiming. 0.5 is the smallest the number can be, as anything lower will mean no zoom. 2 is a lot of zoom, so don't go crazy
     * Ammo Limit- The cap on the amount of ammo for the specific gun the player can carry. Its recommended you put this as a multiple of Max Magazine Size
     * Damage Fall Off- As of 7/10/17, this is not being used by any of the Shoot Scripts. The idea behind it is that the further you are away from an enemy, the less damage you
     * do to them. However, you do full damage all the time, so don't worry about this. 
     * Crit Hit Multiplier- The amount the damage is multiplied by when it's a critical hit AKA headshot
     * Current Magazine- Not required by any of the shoots. However, if you do decide to set this, when the weapon initially spawns in, the current ammo will be set to that.
     * If you set it to higher than the max magazine size, then it will just be the max magazine size
     * Projectile Speed- The speed at which the projectile is fired at. Use higher numbers if you want the weight to be higher
     * Projectile Weight- The amount of mass a projectile has, affects it's speed when fired and how much gravity effects it
     * Projectile- The projectile that is spawned and shot. The projectile should have a rigidbody and a mesh collider attached to it, and if you want it to do anything, you should
     * attach a script as well
     * Blast Radius- The area of effect distance for the projectile explosion. Bigger radius means it can effect more things
     * Num of Pellets- The amount of pellets that are shot by the shotgun. More pellets means more of a chance to hit the enemy
     * 
     * 
     * 
     * The Default Weapon Pickup should have a few things attached: PickupProperties, WeaponProperties, RigidBody, and Box collider.
     * 
     * PickupProperties is similar to the Default Weapon's PickupProperties, ammo should be set to the Ammo gameObject that is the child of DefaultWeaponPickup, and actualGunPrefab
     * should be set to DefaultWeapon. 
     * 
     * You don't need to mess with WeaponProperties, unless you want this weapon to spawn on the floor of a level. At that point, look above to set that.
     * 
     * Rigidbody is a way for the object to interact with unity's physics system. Use Gravity should be checked, and collisionDetection should be Continuous dynamic. Make sure
     * you have a convex mesh collider attached to the 3d model you have downloaded
     * 
     * The box collider should generously surround the shape of the gun, and be set to Trigger.
     */
}
